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
            await _context.Rediles.AsNoTracking()
                .OrderBy(r => r.NumCourse)
                .Select(r => new RedilListDto(r.Id, r.Name, r.NumCourse))
                .ToListAsync();

        public async Task Add(Redile redil) =>
            await _context.Rediles.AddAsync(redil);

        public async Task<RedilDetailsDto?> GetRedilById(int id)
        {
            var redil = await _context.Rediles
                .Include(r => r.Users)
                .Where(r => r.Id == id)
                .Select(r => new RedilDetailsDto(
                    r.Id, r.Name, r.Description ?? "", r.Code, r.NumCourse,
                    r.Users.Where(u => u.IsActive)
                            .Select(u => new RedilTeacherList(u.Id, u.Name, u.Email)
                )))
                .FirstOrDefaultAsync();

            return redil;
        }

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

        public async Task<bool> DoesRedilExists(int id) =>
            await _context.Rediles.AnyAsync(r => r.Id == id);

        public async Task<string?> GetRedilCodeById(int id) =>
            await _context.Rediles.Where(r => r.Id == id).Select(r => r.Code).FirstOrDefaultAsync();

        public async Task<Redile?> GetRedilByCode(string code) =>
            await _context.Rediles.FirstOrDefaultAsync(r => r.Code.Equals(code));

        public async Task<Redile?> GetRedil(int id) =>
            await _context.Rediles.FirstOrDefaultAsync(r => r.Id == id);

        public async Task Update(Redile redil)
        {
            _context.Rediles.Attach(redil);
            _context.Entry(redil).State = EntityState.Modified;
        }
    }
}
