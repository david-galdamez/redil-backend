using Microsoft.EntityFrameworkCore;
using redil_backend.Dtos.Redil;
using redil_backend.Models;

namespace redil_backend.Repository.Redil
{
    public class RedilRepository : IRedilRepository<Redile>
    {

        RedilDbContext _context;

        public RedilRepository(RedilDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RedilListDto>> GetAllRediles() =>
            await _context.Rediles.AsNoTracking().Select(r => new RedilListDto(r.Id, r.Name)).ToListAsync();

        public async Task Add(Redile redil) =>
            await _context.Rediles.AddAsync(redil);

        public async Task<Redile?> GetRedilById(int id) =>
            await _context.Rediles.FindAsync(id);

        public async Task<Redile?> GetRedilByName(string name) =>
            await _context.Rediles.FirstOrDefaultAsync(r => r.Name.Equals(name));

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task<bool> DoesRedilExists(string code) =>
            await _context.Rediles.AnyAsync(r => r.Code.Equals(code));

        public async Task<int?> GetRedilIdByCode(string code)
        {
            return await _context.Rediles
                .Where(r => r.Code.Equals(code))
                .Select(r => (int?)r.Id)
                .FirstOrDefaultAsync();
        }
    }
}
