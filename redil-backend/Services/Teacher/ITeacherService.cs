namespace redil_backend.Services.Teacher
{
    public interface ITeacherService<T, Tr>
    {
        Task<T> RegisterTeacher(Tr registerTeacherDto);
        Task<bool> ValidateTeacher(string email);
    }
}
