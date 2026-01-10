using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using redil_backend.Domain.Enums;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Responses;
using redil_backend.Services;
using redil_backend.Services.Classes;

namespace redil_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private IValidator<RegisterClassDto> _registerClassValidator;
        private IClassService<ServiceResult<ClassDto>, RegisterClassDto> _classService;

        public ClassController(IValidator<RegisterClassDto> registerClassValidator, IClassService<ServiceResult<ClassDto>, RegisterClassDto> classService)
        {
            _registerClassValidator = registerClassValidator;
            _classService = classService;
        }

        [Authorize(Roles = nameof(UserRole.Maestro))]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<ClassDto>>> RegisterClass([FromBody]RegisterClassDto registerClassDto)
        {
            var validationResult = await _registerClassValidator.ValidateAsync(registerClassDto);
            if(!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<ClassDto>
                {
                    Success = false,
                    Message = "Errores de validacion.",
                    Errors = validationResult.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList()
                });
            }

            var redilId = User.GetRedilId();
            if(!redilId.HasValue)
            {
                return Unauthorized(new ApiResponse<ClassDto>
                {
                    Success = false,
                    Message = "No tienes permiso para registrar una clase."
                });
            }
            var teacherId = User.GetUserId();
            var registerResult = await _classService.RegisterClass(registerClassDto, redilId.Value, teacherId);
            if(!registerResult.Success || registerResult.Data == null)
            {
                return BadRequest(new ApiResponse<ClassDto>
                {
                    Success = false,
                    Message = registerResult.ErrorMessage
                });
            }

            return Ok(new ApiResponse<ClassDto>
            {
                Success = true,
                Message = "Clase creada con exito."
            });
        }
    }
}
