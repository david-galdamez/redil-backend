using redil_backend.Dtos.Auth;

namespace redil_backend.Services.Auth
{
    public interface IAuthService<T, Tr, Tl>
    {
        Task<T> Register(Tr authRegisterDto);
        Task<ServiceResult<AuthLoginResult>> Login(Tl authLoginDto);

        Task<bool> ValidateEmail(string email);
    }
}
