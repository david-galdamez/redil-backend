namespace redil_backend.Dtos.Auth
{
    public record AuthRegisterDto(
        string Name,
        string Email,
        string Password);
}
