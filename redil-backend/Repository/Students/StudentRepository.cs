using Microsoft.EntityFrameworkCore;
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

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task<bool> ValidateStudent(string email) =>
            await _context.Students.AnyAsync(s => s.Email.Equals(email));
    }
}
