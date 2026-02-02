namespace redil_backend.Repository.ClassDetails
{
    public interface IClassDetailsRepository<T>
    {
        Task<T?> GetClassDetail(int classId, int studentId);
        Task<ICollection<T>> GetClassDetailsForStats(int? redilId, DateTime fromDate, DateTime toDate, int? groupId);
        Task Update(T classDetails);
        Task Add(T classDetails);
        Task Save();
    }
}
