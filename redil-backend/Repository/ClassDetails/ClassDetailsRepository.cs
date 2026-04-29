using Microsoft.EntityFrameworkCore;
using redil_backend.Dtos;
using redil_backend.Models;

namespace redil_backend.Repository.ClassDetails
{
    public class ClassDetailsRepository : IClassDetailsRepository<ClassDetail>
    {
        private RedilDbContext _context;

        public ClassDetailsRepository(RedilDbContext context)
        {
            _context = context;
        }

        public async Task Add(ClassDetail classDetails) =>
            await _context.ClassDetails.AddAsync(classDetails);

        public async Task<ClassDetail?> GetClassDetail(int classId, int studentId) =>
            await _context.ClassDetails.FirstOrDefaultAsync(cd => cd.StudentId == studentId && cd.ClassId == classId);

        public async Task<ICollection<ClassDetail>> GetClassDetailsForStats(int? redilId, DateTime fromDate, DateTime toDate, int? groupId, string? search)
        {

            var salvadorTz = TimeZoneInfo.FindSystemTimeZoneById("America/El_Salvador");

            var fromUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(fromDate.Date, DateTimeKind.Unspecified), salvadorTz);

            var toUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(toDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified), salvadorTz);

            var query = _context.ClassDetails
                    .Include(cd => cd.Class).ThenInclude(c => c.Redil)
                    .Include(cd => cd.Student).ThenInclude(s => s.Group)
                    .Where(cd =>
                        cd.Class.ClassDate >= fromUtc &&
                        cd.Class.ClassDate <= toUtc &&
                        cd.Student.StudentRedils.Any(sr => 
                            sr.Active &&
                            sr.RedilId == cd.Class.RedilId &&
                            cd.Class.ClassDate >= sr.JoinedAt
                        )
                    );

            if (redilId.HasValue)
            {
                query = query.Where(cd => cd.Class.RedilId == redilId.Value);
            }

            if (groupId.HasValue)
            {
                query = query.Where(cd => cd.Student.GroupId == groupId.Value);
            }

            if(search != null)
            {
                query = query.Where(cd => cd.Student.Name.Contains(search));
            }

            return await query.OrderBy(cd => cd.Class.ClassDate).ToListAsync();
        }

        public async Task<int> GetTotalClassesCount(int? redilId, DateTime fromDate, DateTime toDate)
        {
            var query = _context.Classes
                .Where(c => c.ClassDate >= fromDate && c.ClassDate <= toDate);

            if (redilId.HasValue)
                query = query.Where(c => c.RedilId == redilId.Value);

            return await query.Select(c => c.ClassDate.Date).Distinct().CountAsync();
        }

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task Update(ClassDetail classDetails)
        {
            _context.ClassDetails.Attach(classDetails);
            _context.Entry(classDetails).State = EntityState.Modified;
        }
    }
}
