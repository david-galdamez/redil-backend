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
        private ITeacherService<ServiceResult<TeacherDto>, RegisterTeacherDto, UpdateTeacherDto> _teacherService;
        private IValidator<UpdateTeacherDto> _updateTeacherValidator;

        public TeacherController(
            IValidator<RegisterTeacherDto> registerTeacherValidator, 
            ITeacherService<ServiceResult<TeacherDto>, RegisterTeacherDto, UpdateTeacherDto> teacherService,
            IValidator<UpdateTeacherDto> updateTeacherValidator)
        {
            _registerTeacherValidator = registerTeacherValidator;
            _teacherService = teacherService;
            _updateTeacherValidator = updateTeacherValidator;
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TeacherListDto>>>> GetTeachers()
        {
            var teachers = await _teacherService.GetTeachers();

            return Ok(new ApiResponse<IEnumerable<TeacherListDto>>
            {
                Success = true,
                Data = teachers.Data
            });
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> GetTeacherInfo([FromRoute]int id)
        {
            if(id == 0)
            {
                return BadRequest(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = "Id del maestro no proporcionado"
                });
            }

            var teacherExists = await _teacherService.TeacherExists(id);
            if(!teacherExists)
            {
                return NotFound(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = "Maestro no existe"
                });
            }

            var teacherResult = await _teacherService.GetTeacher(id);

            return Ok(new ApiResponse<TeacherDto>
            {
                Success = true,
                Data = teacherResult.Data
            });
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> UpdateTeacher([FromRoute] int id, [FromBody] UpdateTeacherDto updateTeacherDto)
        {
            if (id == 0)
            {
                return BadRequest(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = "Id del maestro no proporcionado"
                });
            }

            var teacherExists = await _teacherService.TeacherExists(id);
            if (!teacherExists)
            {
                return NotFound(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = "Maestro no existe"
                });
            }

            var validationResult = await _updateTeacherValidator.ValidateAsync(updateTeacherDto);
            if(!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = "Error de validación",
                    Errors = validationResult.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList()
                });
            }

            var updateResult = await _teacherService.UpdateTeacher(updateTeacherDto, id);
            if(!updateResult.Success || updateResult.Data == null)
            {
                return BadRequest(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = updateResult.ErrorMessage ?? "Error al actualizar el maestro."
                });
            }

            return Ok(new ApiResponse<TeacherDto>
            {
                Success = true,
                Data = updateResult.Data
            });
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

            var teacherExists = await _teacherService.TeacherExists(registerTeacherDto.Email);
            if(teacherExists)
            {
                return Conflict(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = "Correo electronico ya registrado."
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
