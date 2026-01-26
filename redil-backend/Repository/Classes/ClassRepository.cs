using Microsoft.EntityFrameworkCore;
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

        public async Task<ICollection<ClassListDto>> GetClasses(int teacherId, int page)
        {
            var query = _context.Classes
                .Where(c => c.TeacherId == teacherId).OrderByDescending(c => c.ClassDate);

            var pageSize = 10;
            var recordsToSkip = (page - 1) * pageSize;

            var paginatedClasses = await query.Skip(recordsToSkip)
                .Take(pageSize)
                .Select(c => new ClassListDto(c.Id, c.Redil.Name, c.ClassDescription, c.ClassDate))
                .ToListAsync();

            return paginatedClasses;
        }

        public async Task Save() =>
            await _context.SaveChangesAsync();
    }
}
