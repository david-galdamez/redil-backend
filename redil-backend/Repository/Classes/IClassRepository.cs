using redil_backend.Dtos;
using redil_backend.Dtos.Classes;

namespace redil_backend.Repository.Classes
{
    public interface IClassRepository<T>
    {
        Task<PaginatedResponse<ClassListDto>> GetClasses(int redilId, int page);
        Task<bool> Exists(int classId);
        Task<bool> Exists(string attendanceToken);
        Task Add(T classes);
        Task Update(T classes);
        Task<T?> GetById(int classId);
        Task<T?> GetByAttendanceToken(string attendanceToken);
        Task Save();
    }
}
