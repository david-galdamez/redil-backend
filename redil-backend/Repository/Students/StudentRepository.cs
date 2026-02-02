using Microsoft.EntityFrameworkCore;
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

        public async Task<Student?> GetStudentByEmail(string email, int redilId) =>
            await _context.Students.Where(s => 
            s.Email.Equals(email) && 
            s.StudentRedils.Any(sr => 
            sr.RedilId == redilId && sr.Active)).FirstOrDefaultAsync();

        public async Task<IEnumerable<StudentListDto>> GetStudentsByRedilId(int redilId)
        {
            return await _context.Students
                .Where(s => s.StudentRedils.Any(sr => sr.RedilId == redilId))
                .Select(s => new StudentListDto(s.Id, s.Name, s.Group.Name, s.IsServer)).ToListAsync();
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
