namespace redil_backend.Repository.Classes
{
    public interface IClassRepository<T>
    {
        Task Add(T classes);
        Task Save();
    }
}
