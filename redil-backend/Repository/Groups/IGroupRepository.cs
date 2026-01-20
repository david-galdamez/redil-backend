namespace redil_backend.Repository.Groups
{
    public interface IGroupRepository<T>
    {
        Task<bool> ValidateGroup(int id);
        Task Add(T entity);
        Task Save();
    }
}
