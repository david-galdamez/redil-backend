using redil_backend.Dtos;
using redil_backend.Dtos.Student;

namespace redil_backend.Services.Students
{
    public interface IStudentService<T, Tr>
    {
        Task<ServiceResult<PaginatedResponse<StudentListDto>>> GetStudentByRedil(int id, int page, string search);
        Task<T> RegisterStudent(Tr registerStudentDto, string code);
        Task<ServiceResult<bool>> FinishCourse(int redilId);
    }
}
