using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Responses;
using redil_backend.Services.Auth;

namespace redil_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAuthService<UserDto, AuthRegisterDto, AuthLoginDto> _authService;
        private IValidator<AuthLoginDto> _loginValidator;

        public AuthController(IAuthService<UserDto, AuthRegisterDto, AuthLoginDto> authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Login(AuthLoginDto authLoginDto)
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

            var userDto = await _authService.Login(authLoginDto);
            if (userDto == null)
            {
                return Unauthorized(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Credenciales inválidas",
                });
            }

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Login exitoso",
                Data = userDto
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

            return Ok();
        }
    }
}
