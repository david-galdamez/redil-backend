using redil_backend.Dtos;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Redil;

namespace redil_backend.Services.Classes
{
    public interface IClassService<T, Tr>
    {
        Task<T> RegisterClass(Tr registerClassDto, int redilId, int teacherId);
        Task<ServiceResult<PaginatedResponse<ClassListDto>>> GetClasses(int teacherId, int page);
        Task<bool> ClassExists(int classId);
        Task<bool> ClassExists(string attendanceToken);
        Task<ServiceResult<AssistStatusDto>> GetAssistStatus(string attendanceToken);
        Task<ServiceResult<ClassDetailsDto>> GetClassDetail(int classId);
        Task<ServiceResult<string>> PassAssist(int classId);
        Task<bool> AssistTokenExists(string attendanceToken);
        Task<bool> ValidateAssistToken(string attendanceToken);
        Task<T> RegisterAssist(string attendanceToken, RegisterAttendanceDto registerAttendanceDto);
        Task<ServiceResult<PaginatedResponse<RedilClassStatDto>>> GetRedilStats(int? redilId, ClassStatsRequestDto classStatsRequest, int page);
    }
}
