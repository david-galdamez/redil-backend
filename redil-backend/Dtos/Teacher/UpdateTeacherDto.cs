namespace redil_backend.Dtos.Teacher
{
    public record UpdateTeacherDto(string Name, string Email, int? RedilId, bool IsActive);
}
