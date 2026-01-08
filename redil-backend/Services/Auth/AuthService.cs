using Microsoft.AspNetCore.Identity;
using redil_backend.Dtos.Auth;
using redil_backend.Mappers.Auth;
using redil_backend.Models;
using redil_backend.Repository.Auth;

namespace redil_backend.Services.Auth
{
    public class AuthService : IAuthService<ServiceResult<UserDto>,AuthRegisterDto, AuthLoginDto>
    {
        private IAuthRepository<users> _authRepository;
        private IPasswordHasher<users> _passwordHasher;
        private TokenProvider _tokenProvider;

        public AuthService(IAuthRepository<users> authRepository, IPasswordHasher<users> passwordHasher, TokenProvider tokenProvider)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
            _tokenProvider = tokenProvider;
        }

        public Task<ServiceResult<UserDto>> Register(AuthRegisterDto authRegisterDto)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult<AuthLoginResult>> Login(AuthLoginDto authLoginDto)
        {
            var user = await _authRepository.GetUserByEmail(authLoginDto.Email);
            if( user == null)
            {
                return ServiceResult<AuthLoginResult>.Fail("Invalid email.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.password, authLoginDto.Password); 
            if(result == PasswordVerificationResult.Failed)
            {
                return ServiceResult<AuthLoginResult>.Fail("Invalid password.");
            }

            var userDto = user.ToUserDto();

            var token = _tokenProvider.CreateToken(userDto);

            var loginResult = new AuthLoginResult(userDto, token);

            return ServiceResult<AuthLoginResult>.Ok(loginResult);
        }
    }
}
