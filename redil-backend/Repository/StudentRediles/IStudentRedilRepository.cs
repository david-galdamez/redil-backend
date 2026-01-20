using redil_backend.Models;

namespace redil_backend.Repository.StudentRediles
{
    public interface IStudentRedilRepository<T>
    {
        Task Add(T entity);
        Task Update(T entity);
        Task<StudentRedil?> GetActiveRelation(int studentId);
        Task Save();
    }
}
