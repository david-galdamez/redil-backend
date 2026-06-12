using redil_backend.Dtos;
using redil_backend.Dtos.Teacher;

namespace redil_backend.Services.Teacher
{
    public interface ITeacherService<T, Tr, Tu>
    {
        Task<T> RegisterTeacher(Tr registerTeacherDto);
        Task<T> UpdateTeacher(Tu updateTeacherDto, int id);
        Task<T> GetTeacher(int id);
        Task<bool> TeacherExists(string email);
        Task<bool> TeacherExists(int id);
        Task<ServiceResult<PaginatedResponse<TeacherListDto>>> GetTeachers(int page, string search, int? redilId = null, int? roleId = null);
        Task<ServiceResult<TeacherDto>> ChangeTeacherPassword(int teacherId, string newPassword);
    }
}
