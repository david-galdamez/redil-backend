using Microsoft.EntityFrameworkCore;
using redil_backend.Dtos.Groups;
using redil_backend.Models;

namespace redil_backend.Repository.Groups
{
    public class GroupRepository : IGroupRepository<Group>
    {
        private RedilDbContext _context;

        public GroupRepository(RedilDbContext context)
        {
            _context = context;
        }

        public async Task Add(Group entity) =>
            await _context.Groups.AddAsync(entity);

        public async Task<ICollection<GroupsListDto>> GetAll() =>
            await _context.Groups.Select(g => new GroupsListDto(g.Id, g.Name)).ToListAsync();

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task<bool> ValidateGroup(int id) =>
            await _context.Groups.AnyAsync(g => g.Id == id);
    }
}
