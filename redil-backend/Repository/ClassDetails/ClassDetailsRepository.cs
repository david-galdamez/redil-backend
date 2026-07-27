using Microsoft.EntityFrameworkCore;
using redil_backend.Dtos;
using redil_backend.Dtos.Redil;
using redil_backend.Models;

namespace redil_backend.Repository.ClassDetails
{
    public class ClassDetailsRepository : IClassDetailsRepository<ClassDetail>
    {
        private readonly RedilDbContext _context;

        public ClassDetailsRepository(RedilDbContext context)
        {
            _context = context;
        }

        public async Task Add(ClassDetail classDetails) =>
            await _context.ClassDetails.AddAsync(classDetails);

        public async Task<ClassDetail?> GetClassDetail(int classId, int studentId) =>
            await _context.ClassDetails.FirstOrDefaultAsync(cd => cd.StudentId == studentId && cd.ClassId == classId);

        public async Task<IEnumerable<string>> GetPhonesByClassId(int classId) =>
            await _context.ClassDetails
                .Where(cd => cd.ClassId == classId)
                .Select(cd => cd.Student.Phone ?? "")
                .ToListAsync();

        public async Task<PaginatedResponse<RedilClassStatDto>> GetClassStatsPaged(
            int? redilId, DateTime fromDate, DateTime toDate, int? groupId, string? search, int page, int pageSize)
        {
            var rows = await FetchStatsRows(redilId, fromDate, toDate, groupId, search);
            var allStats = AggregateStats(rows);

            var totalRecords = allStats.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            var paged = allStats.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedResponse<RedilClassStatDto>
            {
                Data = paged,
                TotalRecords = totalRecords,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task<IReadOnlyList<RedilClassStatDto>> GetClassStatsAll(
            int? redilId, DateTime fromDate, DateTime toDate, int? groupId, string? search)
        {
            var rows = await FetchStatsRows(redilId, fromDate, toDate, groupId, search);
            return AggregateStats(rows);
        }

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task Update(ClassDetail classDetails)
        {
            _context.ClassDetails.Attach(classDetails);
            _context.Entry(classDetails).State = EntityState.Modified;
        }

        // --- private helpers ---

        private sealed record StatsRow(
            int StudentId, string StudentName, string GroupName,
            int RedilId, string RedilName, int NumCourse, bool IsServer,
            DateTime ClassDate, bool Attendance);

        private async Task<List<StatsRow>> FetchStatsRows(
            int? redilId, DateTime fromDate, DateTime toDate, int? groupId, string? search)
        {
            var salvadorTz = TimeZoneInfo.FindSystemTimeZoneById("America/El_Salvador");
            var fromUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(fromDate.Date, DateTimeKind.Unspecified), salvadorTz);
            var toUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(toDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified), salvadorTz);

            var query = _context.ClassDetails
                .Where(cd => cd.Class.ClassDate >= fromUtc && cd.Class.ClassDate <= toUtc);

            if (redilId.HasValue)
            {
                // Con filtro de redil: asistencia de ese redil específico
                // en el periodo en que el estudiante estuvo inscrito en él
                query = query.Where(cd =>
                    cd.Class.RedilId == redilId.Value &&
                    cd.Student.StudentRedils.Any(sr =>
                        sr.RedilId == redilId.Value &&
                        sr.JoinedAt <= cd.Class.ClassDate));
            }
            else
            {
                // Sin filtro: todos los rediles en que estuvo el estudiante durante el periodo
                // Se valida que estuviera inscrito en ese redil al momento de la clase
                query = query.Where(cd =>
                    cd.Student.StudentRedils.Any(sr =>
                        sr.RedilId == cd.Class.RedilId &&
                        sr.JoinedAt <= cd.Class.ClassDate));
            }

            if (groupId.HasValue)
                query = query.Where(cd => cd.Student.GroupId == groupId.Value);

            if (search != null)
                query = query.Where(cd => cd.Student.Name.ToLower().Contains(search.ToLower()));

            // Proyección mínima: sin .Include(), solo las columnas necesarias
            return await query
                .Select(cd => new StatsRow(
                    cd.StudentId,
                    cd.Student.Name,
                    cd.Student.Group.Name,
                    cd.Class.RedilId,
                    cd.Class.Redil.Name,
                    cd.Class.Redil.NumCourse,
                    cd.Student.IsServer,
                    cd.Class.ClassDate,
                    cd.Attendance))
                .ToListAsync();
        }

        private static List<RedilClassStatDto> AggregateStats(List<StatsRow> rows)
        {
            return rows
                .GroupBy(r => r.StudentId)
                .Select(g =>
                {
                    var first = g.First();
                    // Cuando hay filtro de redil todas las filas tienen el mismo RedilId,
                    // por lo que Rediles tendrá un solo elemento.
                    // Sin filtro, Rediles puede tener varios si el estudiante cambió de redil.
                    var rediles = g.GroupBy(r => r.RedilId)
                        .OrderBy(rg => rg.First().NumCourse)
                        .Select(rg => rg.First().RedilName)
                        .ToList();
                    // (date, redilId) para no contar doble si hubiera sesiones el mismo día en distintos rediles
                    var total = g.Select(r => (r.ClassDate.Date, r.RedilId)).Distinct().Count();
                    var attended = g.Count(r => r.Attendance);
                    var pct = total == 0 ? 0f : MathF.Round((float)attended / total * 100, 1);
                    return new RedilClassStatDto(first.StudentName, first.GroupName, rediles, first.IsServer, pct);
                })
                .ToList();
        }
    }
}
