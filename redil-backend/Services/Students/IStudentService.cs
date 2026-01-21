using redil_backend.Dtos.Student;

namespace redil_backend.Services.Students
{
    public interface IStudentService<T, Tr>
    {
        Task<ServiceResult<IEnumerable<StudentListDto>>> GetStudentByRedil(int id);
        Task<T> RegisterStudent(Tr registerStudentDto, string code);
    }
}
