using Microsoft.AspNetCore.Identity;
using redil_backend.Dtos.Auth;
using redil_backend.Mappers.Auth;
using redil_backend.Models;
using redil_backend.Repository.Auth;

namespace redil_backend.Services.Auth
{
    public class AuthService : IAuthService<UserDto, AuthRegisterDto, AuthLoginDto>
    {
        private IAuthRepository<users> _authRepository;
        private IPasswordHasher<users> _passwordHasher;

        public AuthService(IAuthRepository<users> authRepository, IPasswordHasher<users> passwordHasher)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
        }

        public Task<UserDto> Register(AuthRegisterDto authRegisterDto)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto?> Login(AuthLoginDto authLoginDto)
        {
            var user = await _authRepository.GetUserByEmail(authLoginDto.Email);
            if( user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.password, authLoginDto.Password); 
            if(result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var userDto = user.ToUserDto();

            return userDto;
        }


        public void LogOut()
        {
            throw new NotImplementedException();
        }
    }
}
