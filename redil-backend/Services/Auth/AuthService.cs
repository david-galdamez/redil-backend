using Microsoft.AspNetCore.Identity;
using redil_backend.Domain.Enums;
using redil_backend.Dtos.Auth;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.Auth;

namespace redil_backend.Services.Auth
{
    public class AuthService : IAuthService<ServiceResult<UserDto>, AuthRegisterDto, AuthLoginDto>
    {
        private IAuthRepository<User> _authRepository;
        private IPasswordHasher<User> _passwordHasher;
        private TokenProvider _tokenProvider;

        public AuthService(IAuthRepository<User> authRepository, IPasswordHasher<User> passwordHasher, TokenProvider tokenProvider)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
            _tokenProvider = tokenProvider;
        }

        public async Task<ServiceResult<UserDto>> Register(AuthRegisterDto authRegisterDto)
        {
            var user = authRegisterDto.ToUserModel(UserRole.Admin);

            var hashedPassword = _passwordHasher.HashPassword(user, authRegisterDto.Password);

            user.Password = hashedPassword;

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
                return ServiceResult<AuthLoginResult>.Fail("Correo invalido o no existe.");
            }

            if(!user.IsActive)
            {
                return ServiceResult<AuthLoginResult>.Fail("Usuario inactivo. Contacta al administrador.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, authLoginDto.Password); 
            if(result == PasswordVerificationResult.Failed)
            {
                return ServiceResult<AuthLoginResult>.Fail("Contraseña incorrecta.");
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

        public async Task<ServiceResult<LogedUserDto>> GetUserById(int id)
        {
            var user = await _authRepository.GetUserById(id);
            if(user == null)
            {
                return ServiceResult<LogedUserDto>.Fail("Usuario no encontrado.");
            }

            var logedUserDto = user.ToLogedUserDto();

            return ServiceResult<LogedUserDto>.Ok(logedUserDto);
        }

        public async Task<ServiceResult<UserDetailsDto>> GetUserDetailsById(int id)
        {
            var userDetails = await _authRepository.GetUserDetailsById(id);
            if (userDetails == null)
            {
                return ServiceResult<UserDetailsDto>.Fail("Usuario no encontrado.");
            }

            return ServiceResult<UserDetailsDto>.Ok(userDetails);
        }

        public async Task<ServiceResult<UserDto>> UpdateUserDetails(int id, UserProfileUpdateDto userProfileDto)
        {
            var user = await _authRepository.GetUserById(id);
            if(user == null)
            {
                return ServiceResult<UserDto>.Fail("Usuario no encontrado.");
            }

            user.Name = userProfileDto.Name;

            await _authRepository.Update(user);
            await _authRepository.Save();

            return ServiceResult<UserDto>.Ok(user.ToUserDto());
        }

        public async Task<ServiceResult<UserDto>> ChangePassword(int id, UserPasswordChangeDto userPasswordChangeDto)
        {
            var user = await _authRepository.GetUserById(id);
            if (user == null)
            {
                return ServiceResult<UserDto>.Fail("Usuario no encontrado.");
            }

            var validPassword = _passwordHasher.VerifyHashedPassword(user, user.Password, userPasswordChangeDto.CurrentPassword);
            if(validPassword == PasswordVerificationResult.Failed)
            {
                return ServiceResult<UserDto>.Fail("Contraseña actual incorrecta.");
            }

            user.Password = _passwordHasher.HashPassword(user, userPasswordChangeDto.NewPassword);

            await _authRepository.Update(user);
            await _authRepository.Save();

            return ServiceResult<UserDto>.Ok(user.ToUserDto());
        }
    }
}
