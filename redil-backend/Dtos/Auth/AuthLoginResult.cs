namespace redil_backend.Dtos.Auth
{
    public record AuthLoginResult(UserDto user, string accessToken);
}
