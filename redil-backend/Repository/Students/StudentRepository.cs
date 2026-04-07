using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using redil_backend.Dtos;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Student;
using redil_backend.Models;
using redil_backend.Repository.Students;

namespace redil_backend.Repository.Students
{
    public class StudentRepository : IStudentRepository<Student>
    {
        private RedilDbContext _context;

        public StudentRepository(RedilDbContext context)
        {
            _context = context;
        }

        public async Task Add(Student student) =>
            await _context.Students.AddAsync(student);

        public async Task<Student?> GetStudentByEmail(string email) =>
            await _context.Students.Where(s => 
            s.Email.Equals(email)).FirstOrDefaultAsync();

        public async Task<Student?> GetStudentByEmail(string email, int redilId)
        {
            email = email.Trim().ToLower();

            return await _context.StudentRediles
                .Where(sr => sr.RedilId == redilId && sr.Active)
                .Select(sr => sr.Student)
                .Where(s => s.Email.ToLower().Equals(email))
                .FirstOrDefaultAsync();
        }

        public async Task<PaginatedResponse<StudentListDto>> GetStudentsByRedilId(int redilId, int page, string search)
        {
            var query = _context.StudentRediles
                    .Where(sr => sr.RedilId == redilId);

            if(!search.IsNullOrEmpty())
            {
                query = query.Where(sr => sr.Student.Name.ToLower().Contains(search.ToLower()));
            }

            query = query.OrderByDescending(sr => sr.Id);

            var pageSize = 10;

            var totalRecords = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var recordsToSkip = (page - 1) * pageSize;

            var data = await query
                .Skip(recordsToSkip)
                .Take(pageSize)
                .Select(c => new StudentListDto(
                    c.Id,
                    c.Student.Name,
                    c.Student.Group.Name,
                    c.Student.IsServer
                ))
                .ToListAsync();

            return new PaginatedResponse<StudentListDto>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = totalPages
            };

        }

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task Update(Student student)
        {
            _context.Students.Attach(student);
            _context.Entry(student).State = EntityState.Modified;
        }

        public async Task<bool> ValidateStudent(string email) =>
            await _context.Students.AnyAsync(s => s.Email.Equals(email));
    }
}
