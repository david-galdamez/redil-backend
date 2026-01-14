namespace redil_backend.Dtos.Student
{
    public record RegisterStudentDto(string Name, string Email, bool IsServer, int GroupId);
}
