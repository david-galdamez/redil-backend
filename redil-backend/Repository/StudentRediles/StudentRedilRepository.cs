using Microsoft.EntityFrameworkCore;
using redil_backend.Models;

namespace redil_backend.Repository.StudentRediles
{
    public class StudentRedilRepository : IStudentRedilRepository<StudentRedil>
    {
        private RedilDbContext _context;

        public StudentRedilRepository(RedilDbContext context)
        {
            _context = context;
        }

        public async Task Add(StudentRedil entity) =>
            await _context.StudentRediles.AddAsync(entity);

        public async Task<StudentRedil?> GetActiveRelation(int studentId) =>
            await _context.StudentRediles.FirstOrDefaultAsync(sr => sr.StudentId == studentId && sr.Active);

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task Update(StudentRedil entity)
        {
            _context.StudentRediles.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
