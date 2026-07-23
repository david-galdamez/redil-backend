using redil_backend.Dtos.Auth;

namespace redil_backend.Services.Auth
{
    public interface IAuthService<T, Tr, Tl>
    {
        Task<T> Register(Tr authRegisterDto);
        Task<ServiceResult<LogedUserDto>> GetUserById(int id);
        Task<ServiceResult<UserDetailsDto>> GetUserDetailsById(int id);
        Task<ServiceResult<AuthLoginResult>> Login(Tl authLoginDto);
        Task<T> UpdateUserDetails(int id, UserProfileUpdateDto userProfileDto);
        Task<T> ChangePassword(int id, UserPasswordChangeDto userPasswordChangeDto);

        Task<bool> ValidateEmail(string email);
    }
}
