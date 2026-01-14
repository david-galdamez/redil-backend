namespace redil_backend.Repository.Student
{
    public interface IStudentRepository<T>
    {
        Task<bool> ValidateStudent(string email); 
        Task Add(T student);
        Task Save();
    }
}
