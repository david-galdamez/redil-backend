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

        public AuthController(IAuthService<ServiceResult<UserDto>, AuthRegisterDto, AuthLoginDto> authService)
        {
            _authService = authService;
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
                    Message = "Errores de validación",
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
                Message = "Login exitoso",
                Data = loginResult.Data.user
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthRegisterDto authRegisterDto)
        {

            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("access_token");

            return Ok();
        }
    }
}
