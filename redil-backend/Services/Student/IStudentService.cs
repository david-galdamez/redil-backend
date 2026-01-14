namespace redil_backend.Services.Student
{
    public interface IStudentService<T, Tr>
    {
        Task<T> RegisterStudent(Tr registerStudentDto, string code);
    }
}
