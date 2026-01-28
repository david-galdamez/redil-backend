namespace redil_backend.Repository.ClassDetails
{
    public interface IClassDetailsRepository<T>
    {
        Task<T?> GetClassDetail(int classId, int studentId);
        Task Update(T classDetails);
        Task Add(T classDetails);
        Task Save();
    }
}
