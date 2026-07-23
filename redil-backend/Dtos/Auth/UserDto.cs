using redil_backend.Domain.Enums;

namespace redil_backend.Dtos.Auth
{
    public record UserDto (int Id, string Name, string Email, UserRole Role, int? RedilId);
}
