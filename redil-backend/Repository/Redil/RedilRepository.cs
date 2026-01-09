using Microsoft.EntityFrameworkCore;
using redil_backend.Dtos.Redil;
using redil_backend.Models;

namespace redil_backend.Repository.Redil
{
    public class RedilRepository : IRedilRepository<rediles>
    {

        RedilDBContext _context;

        public RedilRepository(RedilDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RedilListDto>> GetAllRediles() =>
            await _context.rediles.AsNoTracking().Select(r => new RedilListDto(r.id, r.name)).ToListAsync();

        public async Task Add(rediles redil) =>
            await _context.rediles.AddAsync(redil);

        public async Task<rediles?> GetRedilById(int id) =>
            await _context.rediles.FindAsync(id);

        public async Task<rediles?> GetRedilByName(string name) =>
            await _context.rediles.FirstOrDefaultAsync(r => r.name.Equals(name));

        public async Task Save() =>
            await _context.SaveChangesAsync();
    }
}
