using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using redil_backend.Domain.Enums;
using redil_backend.Dtos;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Teacher;
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

        public async Task<PaginatedResponse<TeacherListDto>> GetAllTeachers(int page, string search)
        {
            var query = _context.Users.AsQueryable();

            if(!search.IsNullOrEmpty())
            {
                query = query.Where(t => t.Name.ToLower().Contains(search.ToLower()));
            }

            query = query.OrderByDescending(t => t.Id);

            var pageSize = 10;

            var totalRecords = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var recordsToSkip = (page - 1) * pageSize;

            var data = await query
                .Skip(recordsToSkip)
                .Take(pageSize)
                .Select(t => new TeacherListDto(
                    t.Id,
                    t.Name,
                    t.Redil == null ? "Sin Redil Asignado" : t.Redil.Name
                ))
                .ToListAsync();

            return new PaginatedResponse<TeacherListDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task<User> GetTeacher(int teacherId) =>
            await _context.Users.Include(u => u.Redil).FirstAsync(u => u.Id == teacherId);

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public async Task<User?> GetUserById(int id) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<UserDetailsDto?> GetUserDetailsById(int id) =>

            await _context.Users.Include(u => u.Redil)
                .Where(u => u.Id == id)
                .Select(u => new UserDetailsDto(u.Name, u.Email, ((UserRole)u.RoleId).ToString(), u.Redil != null ? u.Redil.Name : "No hay redil asignado"))
                .FirstOrDefaultAsync();

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task<bool> TeacherExists(string email) =>
            await _context.Users.AnyAsync(u => u.Email.Equals(email));

        public async Task<bool> TeacherExists(int teacherId) =>
            await _context.Users.AnyAsync(u => u.Id == teacherId);

        public async Task Update(User entity)
        {
            _context.Users.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
