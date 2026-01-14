using Microsoft.EntityFrameworkCore;
using redil_backend.Models;

namespace redil_backend.Repository.Student
{
    public class StudentRepository : IStudentRepository<students>
    {
        private RedilDBContext _context;

        public StudentRepository(RedilDBContext context)
        {
            _context = context;
        }

        public async Task Add(students student) =>
            await _context.students.AddAsync(student);

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task<bool> ValidateStudent(string email) =>
            await _context.students.AnyAsync(s => s.email.Equals(email));
    }
}
