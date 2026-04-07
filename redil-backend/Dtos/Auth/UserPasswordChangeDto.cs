namespace redil_backend.Dtos.Auth
{
    public record UserPasswordChangeDto(string CurrentPassword, string NewPassword);
}
