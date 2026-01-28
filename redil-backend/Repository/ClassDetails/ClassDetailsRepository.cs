using Microsoft.EntityFrameworkCore;
using redil_backend.Models;

namespace redil_backend.Repository.ClassDetails
{
    public class ClassDetailsRepository : IClassDetailsRepository<ClassDetail>
    {
        private RedilDbContext _context;

        public ClassDetailsRepository(RedilDbContext context)
        {
            _context = context;
        }

        public async Task Add(ClassDetail classDetails) =>
            await _context.ClassDetails.AddAsync(classDetails);

        public async Task<ClassDetail?> GetClassDetail(int classId, int studentId) =>
            await _context.ClassDetails.FirstOrDefaultAsync(cd => cd.StudentId == studentId && cd.ClassId == classId);

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task Update(ClassDetail classDetails)
        {
            _context.ClassDetails.Attach(classDetails);
            _context.Entry(classDetails).State = EntityState.Modified;
        }
    }
}
