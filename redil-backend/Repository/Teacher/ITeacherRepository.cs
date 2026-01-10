namespace redil_backend.Repository.Teacher
{
    public interface ITeacherRepository<T>
    {
        Task Add(T teacher);
        Task Save();
    }
}
