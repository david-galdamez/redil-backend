using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using redil_backend.Domain.Enums;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Responses;
using redil_backend.Dtos.Teacher;
using redil_backend.Services;
using redil_backend.Services.Teacher;

namespace redil_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {

        private IValidator<RegisterTeacherDto> _registerTeacherValidator;
        private ITeacherService<ServiceResult<TeacherDto>, RegisterTeacherDto> _teacherService;

        public TeacherController(IValidator<RegisterTeacherDto> registerTeacherValidator, ITeacherService<ServiceResult<TeacherDto>, RegisterTeacherDto> teacherService)
        {
            _registerTeacherValidator = registerTeacherValidator;
            _teacherService = teacherService;
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> RegisterTeacher([FromBody]RegisterTeacherDto registerTeacherDto)
        {
            var validatorResult = await _registerTeacherValidator.ValidateAsync(registerTeacherDto);
            if(!validatorResult.IsValid)
            {
                return BadRequest(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = "Error de validación",
                    Errors = validatorResult.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList()
                });
            }

            var validTeacher = await _teacherService.ValidateTeacher(registerTeacherDto.Email);
            if(!validTeacher)
            {
                return Conflict(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = "Correo ya registrado"
                });
            }

            var registerResult = await _teacherService.RegisterTeacher(registerTeacherDto);
            if(!registerResult.Success || registerResult.Data == null)
            {
                return BadRequest(new ApiResponse<RedilDto>
                {
                    Success = false,
                    Message = registerResult.ErrorMessage ?? "Error al registrar el redil."
                });
            }

            return Ok(new ApiResponse<TeacherDto>
            {
                Success = true,
                Message = "Maestro creado con exito"
            });
        }
    }
}
