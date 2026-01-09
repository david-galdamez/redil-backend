using Microsoft.AspNetCore.Identity;
using redil_backend.Domain.Enums;
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

        public async Task<ServiceResult<UserDto>> Register(AuthRegisterDto authRegisterDto)
        {
            var user = authRegisterDto.ToUserModel(UserRole.Admin);

            var hashedPassword = _passwordHasher.HashPassword(user, authRegisterDto.Password);

            user.password = hashedPassword;

            await _authRepository.Add(user);
            await _authRepository.Save();

            var userDto = user.ToUserDto();

            return ServiceResult<UserDto>.Ok(userDto);;
        }

        public async Task<ServiceResult<AuthLoginResult>> Login(AuthLoginDto authLoginDto)
        {
            var user = await _authRepository.GetUserByEmail(authLoginDto.Email);
            if( user == null)
            {
                return ServiceResult<AuthLoginResult>.Fail("Correo invalido o no existe");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.password, authLoginDto.Password); 
            if(result == PasswordVerificationResult.Failed)
            {
                return ServiceResult<AuthLoginResult>.Fail("Contraseña incorrecta");
            }

            var userDto = user.ToUserDto();

            var token = _tokenProvider.CreateToken(userDto);

            var loginResult = new AuthLoginResult(userDto, token);

            return ServiceResult<AuthLoginResult>.Ok(loginResult);
        }

        public async Task<bool> ValidateEmail(string email)
        {
            var result = await _authRepository.GetUserByEmail(email);

            return result == null;
        }
    }
}
