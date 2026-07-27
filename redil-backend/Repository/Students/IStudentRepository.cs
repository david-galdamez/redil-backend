using redil_backend.Dtos;
using redil_backend.Dtos.Student;
using redil_backend.Models;

namespace redil_backend.Repository.Students
{
    public interface IStudentRepository<T>
    {
        Task<bool> ValidateStudent(string phone);
        Task<Student?> GetStudentByPhone(string phone);
        Task<Student?> GetStudentByPhone(string phone, int redilId);
        Task<PaginatedResponse<StudentListDto>> GetStudentsByRedilId(int redilId, int page, string search);
        Task Add(T student);
        Task Update(T student);
        Task Save();
    }
}
