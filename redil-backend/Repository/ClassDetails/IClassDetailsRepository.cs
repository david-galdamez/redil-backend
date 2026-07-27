using redil_backend.Dtos;
using redil_backend.Dtos.Redil;

namespace redil_backend.Repository.ClassDetails
{
    public interface IClassDetailsRepository<T>
    {
        Task<T?> GetClassDetail(int classId, int studentId);
        Task<PaginatedResponse<RedilClassStatDto>> GetClassStatsPaged(int? redilId, DateTime fromDate, DateTime toDate, int? groupId, string? search, int page, int pageSize);
        Task<IReadOnlyList<RedilClassStatDto>> GetClassStatsAll(int? redilId, DateTime fromDate, DateTime toDate, int? groupId, string? search);
        Task<IEnumerable<string>> GetPhonesByClassId(int classId);
        Task Update(T classDetails);
        Task Add(T classDetails);
        Task Save();
    }
}
