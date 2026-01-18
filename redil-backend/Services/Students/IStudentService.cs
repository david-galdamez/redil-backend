namespace redil_backend.Services.Students
{
    public interface IStudentService<T, Tr>
    {
        Task<T> RegisterStudent(Tr registerStudentDto, string code);
    }
}
