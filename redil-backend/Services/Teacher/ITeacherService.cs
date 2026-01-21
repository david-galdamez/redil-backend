using redil_backend.Dtos.Teacher;

namespace redil_backend.Services.Teacher
{
    public interface ITeacherService<T, Tr>
    {
        Task<T> RegisterTeacher(Tr registerTeacherDto);
        Task<T> GetTeacher(int id);
        Task<bool> TeacherExists(string email);
        Task<bool> TeacherExists(int id);
        Task<ServiceResult<IEnumerable<TeacherListDto>>> GetTeachers();
    }
}
