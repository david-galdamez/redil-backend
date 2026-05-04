using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using redil_backend.Domain.Enums;
using redil_backend.Dtos;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Responses;
using redil_backend.Services;
using redil_backend.Services.Classes;

namespace redil_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private IValidator<RegisterClassDto> _registerClassValidator;
        private IValidator<RegisterAttendanceDto> _registerAttendanceValidator;
        private IClassService<ServiceResult<ClassDto>, RegisterClassDto> _classService;

        public ClassController(
            IValidator<RegisterClassDto> registerClassValidator, 
            IValidator<RegisterAttendanceDto> registerAttendanceValidator,
            IClassService<ServiceResult<ClassDto>, RegisterClassDto> classService,
            IValidator<ClassStatsRequestDto> classStatsRequestValidator)
        {
            _registerClassValidator = registerClassValidator;
            _classService = classService;
            _registerAttendanceValidator = registerAttendanceValidator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ClassListDto>>>> GetClasses([FromQuery]int page)
        {
            var teacherId = User.GetUserId();

            if(page < 1)
            {
                page = 1;
            }

            var classesResult = await _classService.GetClasses(teacherId, page);
            if(!classesResult.Success || classesResult.Data == null)
            {
                return BadRequest(new ApiResponse<PaginatedResponse<ClassListDto>>
                {
                    Success = false,
                    Message = classesResult.ErrorMessage
                });
            }

            return Ok(new ApiResponse<PaginatedResponse<ClassListDto>>
            {
                Success = true,
                Data = classesResult.Data
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ClassDetailsDto>>> GetClassDetail([FromRoute]int id)
        {
            if(id == 0)
            {
                return BadRequest(new ApiResponse<ClassDetailsDto>
                {
                    Success = false,
                    Message = "El id de la clase es invalido."
                });
            }

            var classExists = await _classService.ClassExists(id);
            if(!classExists)
            {
                return NotFound(new ApiResponse<ClassDetailsDto>
                {
                    Success = false,
                    Message = "La clase no existe."
                });
            }

            var classResult = await _classService.GetClassDetail(id);
            if(!classResult.Success || classResult.Data == null)
            {
                return BadRequest(new ApiResponse<ClassDetailsDto>
                {
                    Success = false,
                    Message = classResult.ErrorMessage
                });
            }

            return Ok(new ApiResponse<ClassDetailsDto>
            {
                Success = true,
                Data = classResult.Data
            });
        }

        [AllowAnonymous]
        [HttpGet("assist/{attendanceToken}")]
        public async Task<ActionResult<ApiResponse<AssistStatusDto>>> GetAssistStatus([FromRoute]string attendanceToken)
        {
            if(attendanceToken.IsNullOrEmpty())
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "El id de la clase es invalido."
                });
            }

            var classExists = await _classService.ClassExists(attendanceToken);
            if(!classExists)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "La clase no existe."
                });
            }
            var assistStatusResult = await _classService.GetAssistStatus(attendanceToken);
            if(!assistStatusResult.Success || assistStatusResult.Data == null)
            {
                return BadRequest(new ApiResponse<AssistStatusDto>
                {
                    Success = false,
                    Message = assistStatusResult.ErrorMessage
                });
            }
            return Ok(new ApiResponse<AssistStatusDto>
            {
                Success = true,
                Data = assistStatusResult.Data
            });
        }

        [HttpPut("assist/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> PassAssist([FromRoute]int id)
        {
            if(id == 0)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "El id de la clase es invalido."
                });
            }

            var classExists = await _classService.ClassExists(id);
            if(!classExists)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "La clase no existe."
                });
            }

            var assistResult = await _classService.PassAssist(id);
            if(!assistResult.Success || assistResult.Data == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = assistResult.ErrorMessage
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Asistencia iniciada con exito.",
                Data = assistResult.Data
            });
        }

        [AllowAnonymous]
        [HttpPost("assist/register/{attendanceToken}")]
        public async Task<ActionResult<ApiResponse<string>>> RegisterAssist([FromRoute]string attendanceToken, [FromBody]RegisterAttendanceDto registerAssistDto)
        {
            if(attendanceToken.IsNullOrEmpty())
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "El token de la clase es invalido."
                });
            }
            
            var assistTokenExists = await _classService.AssistTokenExists(attendanceToken);
            if(!assistTokenExists)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "El token de asistencia no existe."
                });
            }

            var classExists = await _classService.ClassExists(attendanceToken);
            if(!classExists)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "La clase no existe."
                });
            }

            var validationResult = await _registerAttendanceValidator.ValidateAsync(registerAssistDto);
            if(!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<string>
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

            var registerResult = await _classService.RegisterAssist(attendanceToken, registerAssistDto);
            if(!registerResult.Success || registerResult.Data == null)
            {

                if(registerResult.ErrorMessage != null && registerResult.ErrorMessage.Contains("expirado"))
                {
                    return Conflict(new ApiResponse<string>
                    {
                        Success = false,
                        Message = registerResult.ErrorMessage
                    });
                }

                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = registerResult.ErrorMessage
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Asistencia registrada con exito.",
            });
        }

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
                Data = registerResult.Data,
                Message = "Clase creada con exito."
            });
        }
    }
}
