using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Responses;
using redil_backend.Services;
using redil_backend.Services.Auth;

namespace redil_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAuthService<ServiceResult<UserDto>, AuthRegisterDto, AuthLoginDto> _authService;
        private IValidator<AuthLoginDto> _loginValidator;
        private IValidator<AuthRegisterDto> _registerValidator;

        public AuthController(
            IAuthService<ServiceResult<UserDto>, AuthRegisterDto, AuthLoginDto> authService,
            IValidator<AuthLoginDto> loginValidator,
            IValidator<AuthRegisterDto> registerValidator
            )
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

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
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Login exitoso.",
            });
        }

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

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Logout()
        {
            Response.Cookies.Delete("access_token");

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Cierre de sesion exitoso.",
            });
        }
    }
}
