using redil_backend.Dtos.Groups;

namespace redil_backend.Services.Groups
{
    public interface IGroupService<T>
    {
        Task<ServiceResult<ICollection<GroupsListDto>>> GetAllGroups();
        Task<ServiceResult<bool>> ValidateGroup(int id);
        Task<T> AddGroup(RegisterGroupDto registerGroupDto);
    }
}
