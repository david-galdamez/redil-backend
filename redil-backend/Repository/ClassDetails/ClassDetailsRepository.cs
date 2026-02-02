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

        public async Task<ICollection<ClassDetail>> GetClassDetailsForStats(int? redilId, DateTime fromDate, DateTime toDate, int? groupId)
        {
            var query = _context.ClassDetails
                    .Include(cd => cd.Class).ThenInclude(c => c.Redil)
                    .Include(cd => cd.Student).ThenInclude(s => s.Group)
                    .Where(cd =>
                        cd.Class.ClassDate >= fromDate &&
                        cd.Class.ClassDate <= toDate &&
                        cd.Student.StudentRedils.Any(sr => sr.Active)
                    );

            if (redilId.HasValue)
            {
                query = query.Where(cd => cd.Class.RedilId == redilId.Value);
            }

            if (groupId.HasValue)
            {
                query = query.Where(cd => cd.Student.GroupId == groupId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task Update(ClassDetail classDetails)
        {
            _context.ClassDetails.Attach(classDetails);
            _context.Entry(classDetails).State = EntityState.Modified;
        }
    }
}
