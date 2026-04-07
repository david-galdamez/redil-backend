using redil_backend.Dtos;

namespace redil_backend.Repository.ClassDetails
{
    public interface IClassDetailsRepository<T>
    {
        Task<T?> GetClassDetail(int classId, int studentId);
        Task<ICollection<T>> GetClassDetailsForStats(int? redilId, DateTime fromDate, DateTime toDate, int? groupId, string? search);
        Task<int> GetTotalClassesCount(int? redilId, DateTime fromDate, DateTime toDate);
        Task Update(T classDetails);
        Task Add(T classDetails);
        Task Save();
    }
}
