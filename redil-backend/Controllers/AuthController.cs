using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using redil_backend.Domain.Enums;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Responses;
using redil_backend.Middlewares;
using redil_backend.Services;
using redil_backend.Services.Auth;
using redil_backend.Validators.Auth;

namespace redil_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService<ServiceResult<UserDto>, AuthRegisterDto, AuthLoginDto> _authService;
        private IValidator<AuthLoginDto> _loginValidator;
        private IValidator<UserProfileUpdateDto> _updateUserValidator;
        private IValidator<AuthRegisterDto> _registerValidator;
        private IValidator<UserPasswordChangeDto> _updatePasswordValidator;

        public AuthController(
            IAuthService<ServiceResult<UserDto>, AuthRegisterDto, AuthLoginDto> authService,
            IValidator<AuthLoginDto> loginValidator,
            IValidator<UserPasswordChangeDto> updatePasswordValidator,
            IValidator<UserProfileUpdateDto> updateUserValidator,
            IValidator<AuthRegisterDto> registerValidator
            )
        {
            _authService = authService;
            _updateUserValidator = updateUserValidator;
            _updatePasswordValidator = updatePasswordValidator;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Login([FromBody]AuthLoginDto authLoginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(authLoginDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Errores de validación.",
                    Errors = validationResult.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList(),
                });
            }

            var loginResult = await _authService.Login(authLoginDto);
            if (!loginResult.Success || loginResult.Data == null)
            {
                return Unauthorized(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = loginResult.ErrorMessage
                });
            }

            HttpContext.Response.Cookies.Append("access_token", loginResult.Data.accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Login exitoso.",
            });
        }

        [ApiKey]
        [HttpPost("register_admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody]AuthRegisterDto authRegisterDto)
        {
            var validationResult = await _registerValidator.ValidateAsync(authRegisterDto);
            if(!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Errores de validación.",
                    Errors = validationResult.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList(),
                });
            }

            var validateEmail = await _authService.ValidateEmail(authRegisterDto.Email);
            if(!validateEmail)
            {
                return Conflict(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "El correo electrónico ya está en uso."
                });
            }

            var registerResult = await _authService.Register(authRegisterDto);
            if(!registerResult.Success || registerResult.Data == null)
            {
                return BadRequest(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = registerResult.ErrorMessage
                });
            }

            return Ok(new ApiResponse<UserDto>
            {
                    Success = true,
                    Message = "Registro exitoso.",
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserDetailsDto>>> GetCurrentUser()
        {
            var userId = User.GetUserId();
            var userResult = await _authService.GetUserDetailsById(userId);
            if (!userResult.Success || userResult.Data == null)
            {
                return NotFound(new ApiResponse<UserDetailsDto>
                {
                    Success = false,
                    Message = "Usuario no encontrado."
                });
            }

            return Ok(new ApiResponse<UserDetailsDto>
            {
                Success = true,
                Message = "Usuario obtenido exitosamente.",
                Data = userResult.Data
            });
        }

        [Authorize]
        [HttpPatch("me")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody]UserProfileUpdateDto profileUpdateDto)
        {

            var updateValidator = await _updateUserValidator.ValidateAsync(profileUpdateDto);
            if(!updateValidator.IsValid)
            {
                return BadRequest(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Errores de validación.",
                    Errors = updateValidator.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList(),
                });
            }

            var userId = User.GetUserId();
            var updateResult = await _authService.UpdateUserDetails(userId, profileUpdateDto);
            if (!updateResult.Success || updateResult.Data == null)
            {
                if(updateResult.ErrorMessage != null && updateResult.ErrorMessage.Contains("no encontrado", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ApiResponse<UserDetailsDto>
                    {
                        Success = false,
                        Message = updateResult.ErrorMessage
                    });
                }
                
                return BadRequest(new ApiResponse<UserDetailsDto>
                {
                    Success = false,
                    Message = updateResult.ErrorMessage ?? "Error al obtener el usuario."
                });
            }
            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Perfil actualizado exitosamente.",
                Data = updateResult.Data
            });
        }

        [Authorize]
        [HttpPatch("me/password")]
        public async Task<ActionResult<ApiResponse<UserDto>>> ChangePassword([FromBody]UserPasswordChangeDto passwordChangeDto)
        {
            var updateValidator = await _updatePasswordValidator.ValidateAsync(passwordChangeDto);
            if(!updateValidator.IsValid)
            {
                return BadRequest(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Errores de validación.",
                    Errors = updateValidator.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList(),
                });
            }

            var userId = User.GetUserId();
            var changeResult = await _authService.ChangePassword(userId, passwordChangeDto);
            if (!changeResult.Success || changeResult.Data == null)
            {
                if(changeResult.ErrorMessage != null && changeResult.ErrorMessage.Contains("no encontrado", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ApiResponse<UserDetailsDto>
                    {
                        Success = false,
                        Message = changeResult.ErrorMessage
                    });
                }
                
                return BadRequest(new ApiResponse<UserDetailsDto>
                {
                    Success = false,
                    Message = changeResult.ErrorMessage ?? "Error al cambiar la contraseña."
                });
            }
            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Contraseña cambiada exitosamente.",
                Data = changeResult.Data
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Logout()
        {
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Cierre de sesion exitoso.",
            });
        }
    }
}
