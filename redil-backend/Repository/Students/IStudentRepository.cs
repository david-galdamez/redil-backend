using redil_backend.Models;

namespace redil_backend.Repository.Students
{
    public interface IStudentRepository<T>
    {
        Task<bool> ValidateStudent(string email);

        Task<Student?> GetStudentByEmail(string email);
        Task Add(T student);
        Task Update(T student);
        Task Save();
    }
}
