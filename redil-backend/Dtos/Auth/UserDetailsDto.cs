namespace redil_backend.Dtos.Auth
{
    public record UserDetailsDto(string Name, string Email, string Role, string RedilName, int? RedilId);
}
