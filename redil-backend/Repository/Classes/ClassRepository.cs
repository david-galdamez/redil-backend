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

        public async Task Save() =>
            await _context.SaveChangesAsync();
    }
}
