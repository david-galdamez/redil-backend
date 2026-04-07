using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using redil_backend.Domain.Enums;
using redil_backend.Dtos;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Responses;
using redil_backend.Services;
using redil_backend.Services.Classes;
using redil_backend.Services.Groups;
using redil_backend.Services.Redil;

namespace redil_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedilController : ControllerBase
    {

        private IValidator<RegisterRedilDto> _registerRedilValidator;
        private IRedilService<ServiceResult<RedilDto>, RegisterRedilDto> _redilService;
        private IValidator<ClassStatsRequestDto> _classStatsRequestValidator;
        private IClassService<ServiceResult<ClassDto>, RegisterClassDto> _classService;
        private IGroupService<ServiceResult<int>> _groupService;

        public RedilController(
            IRedilService<ServiceResult<RedilDto>, RegisterRedilDto> redilService,
            IValidator<RegisterRedilDto> registerRedilValidator,
            IClassService<ServiceResult<ClassDto>, RegisterClassDto> classService,
            IValidator<ClassStatsRequestDto> classStatsRequestValidator,
            IGroupService<ServiceResult<int>> groupService)
        {
            _redilService = redilService;
            _registerRedilValidator = registerRedilValidator;
            _classStatsRequestValidator = classStatsRequestValidator;
            _classService = classService;
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RedilListDto>>>> GetRediles()
        {
            var rediles = await _redilService.GetRediles();

            return Ok(new ApiResponse<IEnumerable<RedilListDto>>
            {
                Success = true,
                Message = "Rediles obtenidos.",
                Data = rediles
            });
        }

        [Authorize]
        [HttpGet("code")]
        public async Task<ActionResult<ApiResponse<string>>> GetRedilCode()
        {

            var redilId = User.GetRedilId();
            if(!redilId.HasValue)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "El usuario no tiene un redil asignado."
                });
            }

            var validRedil = await _redilService.RedilExists(redilId.Value);
            if (!validRedil)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Id del redil no existe."
                });
            }
            var codeResult = await _redilService.GetRedilCode(redilId.Value);
            if (!codeResult.Success || codeResult.Data == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = codeResult.ErrorMessage ?? "Error al obtener el código del redil."
                });
            }
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Código del redil obtenido.",
                Data = codeResult.Data
            });
        }

        [AllowAnonymous]
        [HttpGet("code/{code}")]
        public async Task<ActionResult<ApiResponse<RedilDto>>> GetRedilByCode([FromRoute]string code)
        {
            var redilIdResult = await _redilService.GetRedilByCode(code);
            if (!redilIdResult.Success || redilIdResult.Data == null)
            {
                return NotFound(new ApiResponse<RedilDto>
                {
                    Success = false,
                    Message = redilIdResult.ErrorMessage ?? "Error al obtener el redil por código."
                });
            }
            return Ok(new ApiResponse<RedilDto>
            {
                Success = true,
                Message = "Redil obtenido con exito.",
                Data = redilIdResult.Data
            });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<RedilDetailsDto>> GetRedilById([FromRoute]int id)
        {
            var validRedil = await _redilService.RedilExists(id);
            if (!validRedil)
            {
                return BadRequest(new ApiResponse<RedilDetailsDto>
                {
                    Success = false,
                    Message = "Id del redil no existe."
                });
            }
            var redilResult = await _redilService.GetRedilById(id);
            if (!redilResult.Success || redilResult.Data == null)
            {
                return BadRequest(new ApiResponse<RedilDetailsDto>
                {
                    Success = false,
                    Message = redilResult.ErrorMessage ?? "Error al obtener el redil por id."
                });
            }
            return Ok(new ApiResponse<RedilDetailsDto>
            {
                Success = true,
                Message = "Redil obtenido con exito.",
                Data = redilResult.Data
            });
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost("stats")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<RedilClassStatDto>>>> GetRedilStats([FromQuery]int page, [FromBody]ClassStatsRequestDto classStatsRequest)
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

            if (classStatsRequest.GroupId.HasValue)
            {
                var validGroup = await _groupService.ValidateGroup(classStatsRequest.GroupId.Value);
                if (!validGroup.Success || !validGroup.Data)
                {
                    return BadRequest(new ApiResponse<PaginatedResponse<RedilClassStatDto>>
                    {
                        Success = false,
                        Message = "Id del grupo no existe."
                    });
                }
            }

            if (classStatsRequest.RedilId.HasValue)
            {
                var validRedil = await _redilService.RedilExists(classStatsRequest.RedilId.Value);
                if (!validRedil)
                {
                    return BadRequest(new ApiResponse<PaginatedResponse<RedilClassStatDto>>
                    {
                        Success = false,
                        Message = "Id del grupo no existe."
                    });
                }
            }

            var classStats = await _classService.GetRedilStats(classStatsRequest.RedilId, classStatsRequest, page);
            if (!classStats.Success || classStats.Data == null)
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
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RedilDto>>> RegisterRedil([FromBody]RegisterRedilDto registerRedilDto)
        {
            var validationResult = await _registerRedilValidator.ValidateAsync(registerRedilDto);
            if(!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<RedilDto>
                {
                    Success = false,
                    Message = "Error de validación.",
                    Errors = validationResult.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList()
                });
            }

            var registerResult = await _redilService.RegisterRedil(registerRedilDto);

            if(!registerResult.Success || registerResult.Data == null)
            {
                return BadRequest(new ApiResponse<RedilDto>
                {
                    Success = false,
                    Message = registerResult.ErrorMessage ?? "Error al registrar el redil."
                });
            }

            return Ok(new ApiResponse<RedilDto>
            {
                Success = true,
                Message = "Redil registrado.",
                Data = registerResult.Data
            });
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPut("{id}")]
        public async Task<ActionResult<RedilDetailsDto>> UpdateRedil([FromRoute]int id, [FromBody]RegisterRedilDto updateRedilDto)
        {
            var validatorResult = await _registerRedilValidator.ValidateAsync(updateRedilDto);
            if (!validatorResult.IsValid)
            {
                return BadRequest(new ApiResponse<RedilDetailsDto>
                {
                    Success = false,
                    Message = "Error de validación.",
                    Errors = validatorResult.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList()
                });
            }

            var redilExists = await _redilService.RedilExists(id);
            if (!redilExists)
            {
                return BadRequest(new ApiResponse<RedilDetailsDto>
                {
                    Success = false,
                    Message = "Id del redil no existe."
                });
            }

            var updateResult = await _redilService.UpdateRedil(id, updateRedilDto);
            if (!updateResult.Success || updateResult.Data == null)
            {
                return BadRequest(new ApiResponse<RedilDetailsDto>
                {
                    Success = false,
                    Message = updateResult.ErrorMessage ?? "Error al actualizar el redil."
                });
            }

            return Ok(new ApiResponse<RedilDetailsDto>
            {
                Success = true,
                Message = "Redil actualizado.",
                Data = updateResult.Data
            });
        }
    }
}
