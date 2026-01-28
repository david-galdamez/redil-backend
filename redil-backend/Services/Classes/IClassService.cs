using redil_backend.Dtos.Classes;

namespace redil_backend.Services.Classes
{
    public interface IClassService<T, Tr>
    {
        Task<T> RegisterClass(Tr registerClassDto, int redilId, int teacherId);
        Task<ServiceResult<ICollection<ClassListDto>>> GetClasses(int teacherId, int page);
        Task<bool> ClassExists(int classId);
        Task<bool> ClassExists(string attendanceToken);
        Task<ServiceResult<ClassDetailsDto>> GetClassDetail(int classId);
        Task<ServiceResult<string>> PassAssist(int classId);
        Task<bool> ValidateAssistToken(string attendanceToken);
        Task<T> RegisterAssist(string attendanceToken, RegisterAttendanceDto registerAttendanceDto);
    }
}
