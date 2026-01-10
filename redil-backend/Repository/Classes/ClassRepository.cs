using redil_backend.Models;

namespace redil_backend.Repository.Classes
{
    public class ClassRepository : IClassRepository<classes>
    {
        private RedilDBContext _context;
        
        public ClassRepository(RedilDBContext context)
        {
            _context = context;
        }

        public async Task Add(classes classes) =>
            await _context.classes.AddAsync(classes);

        public async Task Save() =>
            await _context.SaveChangesAsync();
    }
}
