using redil_backend.Dtos.Student;
using redil_backend.Models;

namespace redil_backend.Repository.Students
{
    public interface IStudentRepository<T>
    {
        Task<bool> ValidateStudent(string email);

        Task<Student?> GetStudentByEmail(string email, int redilId);
        Task<IEnumerable<StudentListDto>> GetStudentsByRedilId(int redilId);
        Task Add(T student);
        Task Update(T student);
        Task Save();
    }
}
