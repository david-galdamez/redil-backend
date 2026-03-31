using Microsoft.EntityFrameworkCore;
using redil_backend.Dtos;
using redil_backend.Dtos.Classes;
using redil_backend.Models;

namespace redil_backend.Repository.Classes
{
    public class ClassRepository : IClassRepository<Class>
    {
        private RedilDbContext _context;
        
        public ClassRepository(RedilDbContext context)
        {
            _context = context;
        }

        public async Task Add(Class classes) =>
            await _context.Classes.AddAsync(classes);

        public async Task<bool> Exists(int classId) =>
            await _context.Classes.AnyAsync(c => c.Id == classId);

        public async Task<bool> Exists(string attendanceToken) =>
            await _context.Classes.AnyAsync(c => c.AttendanceToken != null && c.AttendanceToken.Equals(attendanceToken));

        public async Task<Class?> GetByAttendanceToken(string attendanceToken) =>
            await _context.Classes.FirstOrDefaultAsync(c => c.AttendanceToken != null && c.AttendanceToken.Equals(attendanceToken));

        public async Task<Class?> GetById(int classId) =>
            await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);

        public async Task<PaginatedResponse<ClassListDto>> GetClasses(int teacherId, int page)
        {
            var query = _context.Classes
                .Where(c => c.TeacherId == teacherId)
                .OrderByDescending(c => c.ClassDate);

            var pageSize = 10;

            var totalRecords = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var recordsToSkip = (page - 1) * pageSize;

            var data = await query
                .Skip(recordsToSkip)
                .Take(pageSize)
                .Select(c => new ClassListDto(
                    c.Id,
                    c.Redil.Name,
                    c.ClassDescription,
                    c.ClassDate
                ))
                .ToListAsync();

            return new PaginatedResponse<ClassListDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task Update(Class classes)
        {
            _context.Classes.Attach(classes);
            _context.Entry(classes).State = EntityState.Modified;
        }
    }
}
