using redil_backend.Dtos.Groups;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.Groups;

namespace redil_backend.Services.Groups
{
    public class GroupService : IGroupService<ServiceResult<int>>
    {

        private IGroupRepository<Group> _groupRepository;

        public GroupService(IGroupRepository<Group> groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<ServiceResult<bool>> ValidateGroup(int id)
        {
            var isValid = await _groupRepository.ValidateGroup(id);
            return ServiceResult<bool>.Ok(isValid);
        }

        public async Task<ServiceResult<int>> AddGroup(RegisterGroupDto registerGroupDto)
        {
            var group = registerGroupDto.ToGroupModel();

            await _groupRepository.Add(group);
            await _groupRepository.Save();

            return ServiceResult<int>.Ok(group.Id);
        }

        public async Task<ServiceResult<ICollection<GroupsListDto>>> GetAllGroups()
        {
            var groups = await _groupRepository.GetAll();

            return ServiceResult<ICollection<GroupsListDto>>.Ok(groups);
        }
    }
}
