namespace redil_backend.Repository.Students
{
    public interface IStudentRepository<T>
    {
        Task<bool> ValidateStudent(string email); 
        Task Add(T student);
        Task Save();
    }
}
