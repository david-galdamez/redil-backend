using Microsoft.EntityFrameworkCore;
using redil_backend.Models;

namespace redil_backend.Repository.Auth
{
    public class AuthRepository : IAuthRepository<User>
    {
        private RedilDbContext _context;

        public AuthRepository(RedilDbContext context)
        {
            _context = context;
        }

        public async Task Add(User entity) =>
            await _context.Users.AddAsync(entity);

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public async Task Save() =>
            await _context.SaveChangesAsync();
    }
}
