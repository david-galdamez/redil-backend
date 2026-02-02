using redil_backend.Dtos.Groups;

namespace redil_backend.Repository.Groups
{
    public interface IGroupRepository<T>
    {
        Task<bool> ValidateGroup(int id);
        Task<ICollection<GroupsListDto>> GetAll();
        Task Add(T entity);
        Task Save();
    }
}
