using redil_backend.Domain.Enums;

namespace redil_backend.Dtos.Teacher
{
    public record TeacherDto(string Name, string Email, string RedilName, UserRole Role);
}
