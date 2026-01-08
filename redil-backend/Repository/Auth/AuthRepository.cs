using Microsoft.EntityFrameworkCore;
using redil_backend.Models;

namespace redil_backend.Repository.Auth
{
    public class AuthRepository : IAuthRepository<users>
    {
        private RedilDBContext _context;

        public AuthRepository(RedilDBContext context)
        {
            _context = context;
        }

        public async Task Add(users entity) =>
            await _context.users.AddAsync(entity);

        public async Task<users?> GetUserByEmail(string email)
        {
            return await _context.users.FirstOrDefaultAsync(u => u.email.Equals(email));
        }

        public async Task Save() =>
            await _context.SaveChangesAsync();
    }
}
