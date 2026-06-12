using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using redil_backend.Domain.Enums;
using redil_backend.Dtos;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Responses;
using redil_backend.Dtos.Teacher;
using redil_backend.Services;
using redil_backend.Services.Classes;
using redil_backend.Services.Groups;
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
        private IValidator<TeacherPasswordChangeDto> _teacherPasswordChangeValidator;
        private IClassService<ServiceResult<ClassDto>, RegisterClassDto> _classService;
        private IValidator<ClassStatsRequestDto> _classStatsRequestValidator;
        private IGroupService<ServiceResult<int>> _groupService;

        public TeacherController(
            IValidator<RegisterTeacherDto> registerTeacherValidator, 
            ITeacherService<ServiceResult<TeacherDto>, RegisterTeacherDto, UpdateTeacherDto> teacherService,
            IValidator<UpdateTeacherDto> updateTeacherValidator,
            IValidator<TeacherPasswordChangeDto> teacherPasswordChangeValidator,
            IClassService<ServiceResult<ClassDto>, RegisterClassDto> classService,
            IValidator<ClassStatsRequestDto> classStatsRequestValidator,
            IGroupService<ServiceResult<int>> groupService)
        {
            _registerTeacherValidator = registerTeacherValidator;
            _teacherService = teacherService;
            _updateTeacherValidator = updateTeacherValidator;
            _teacherPasswordChangeValidator = teacherPasswordChangeValidator;
            _classService = classService;
            _classStatsRequestValidator = classStatsRequestValidator;
            _groupService = groupService;
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<TeacherListDto>>>> GetTeachers([FromQuery]int page, [FromQuery]string? search, [FromQuery]int? redilId, [FromQuery]int? roleId)
        {
        
            if(page < 1)
            {
                page = 1;
            }

            if(search == null)
            {
                search = string.Empty;
            }

            var teachers = await _teacherService.GetTeachers(page, search, redilId, roleId);
            return Ok(new ApiResponse<PaginatedResponse<TeacherListDto>>
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

        [Authorize]
        [HttpPost("redil/stats")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<RedilClassStatDto>>>> GetClassStats([FromQuery]int page, [FromBody]ClassStatsRequestDto classStatsRequest)
        { 
            if(page < 1)
            {
                page = 1;
            }

            var validationResult = await _classStatsRequestValidator.ValidateAsync(classStatsRequest);
            if(!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<PaginatedResponse<RedilClassStatDto>>
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
                return Unauthorized(new ApiResponse<PaginatedResponse<RedilClassStatDto>>
                {
                    Success = false,
                    Message = "No tienes permiso para ver las estadisticas de la clase."
                });
            }

            if(classStatsRequest.GroupId.HasValue)
            {
                var validGroup = await _groupService.ValidateGroup(classStatsRequest.GroupId.Value);
                if(!validGroup.Success || !validGroup.Data)
                {
                    return BadRequest(new ApiResponse<PaginatedResponse<RedilClassStatDto>>
                    {
                        Success = false,
                        Message = "Id del grupo no existe."
                    });
                }
            }

            var classStats = await _classService.GetRedilStats(redilId.Value, classStatsRequest, page);
            if(!classStats.Success || classStats.Data == null)
            {
                return BadRequest(new ApiResponse<PaginatedResponse<RedilClassStatDto>>
                {
                    Success = false,
                    Message = classStats.ErrorMessage
                });
            }

            return Ok(new ApiResponse<PaginatedResponse<RedilClassStatDto>>
            {
                Success = true,
                Data = classStats.Data
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
        [HttpPost]
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

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPatch("{id}/password")]
        public async Task<ActionResult<ApiResponse<TeacherDto>>> ChangeTeacherPassword([FromRoute] int id, [FromBody] TeacherPasswordChangeDto passwordChangeDto)
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

            var validationResult = await _teacherPasswordChangeValidator.ValidateAsync(passwordChangeDto);
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

            var changeResult = await _teacherService.ChangeTeacherPassword(id, passwordChangeDto.NewPassword);
            if(!changeResult.Success || changeResult.Data == null)
            {
                return BadRequest(new ApiResponse<TeacherDto>
                {
                    Success = false,
                    Message = changeResult.ErrorMessage ?? "Error al cambiar la contraseña."
                });
            }

            return Ok(new ApiResponse<TeacherDto>
            {
                Success = true,
                Message = "Contraseña actualizada exitosamente."
            });
        }
    }
}
