using redil_backend.Dtos.Classes;

namespace redil_backend.Repository.Classes
{
    public interface IClassRepository<T>
    {
        Task<ICollection<ClassListDto>> GetClasses(int teacherId, int page);
        Task Add(T classes);
        Task Save();
    }
}
