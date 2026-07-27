namespace redil_backend.Dtos.Student
{
    public record RegisterStudentDto(string Name, string Phone, bool IsServer, int GroupId, string? Email = null);
}
