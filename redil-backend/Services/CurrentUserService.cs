using Microsoft.EntityFrameworkCore;
using redil_backend.Models;

namespace redil_backend.Services
{
    public class CurrentUserService
    {
        private readonly RedilDbContext _context;

        public CurrentUserService(RedilDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetUserRedilId(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user?.RedilId;
        }
    }
}
