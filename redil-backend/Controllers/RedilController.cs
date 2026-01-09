using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using redil_backend.Domain.Enums;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Responses;
using redil_backend.Services;
using redil_backend.Services.Redil;

namespace redil_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedilController : ControllerBase
    {

        private IValidator<RegisterRedilDto> _registerRedilValidator;
        private IRedilService<ServiceResult<RedilDto>, RegisterRedilDto> _redilService;

        public RedilController(IRedilService<ServiceResult<RedilDto>, RegisterRedilDto> redilService, IValidator<RegisterRedilDto> registerRedilValidator)
        {
            _redilService = redilService;
            _registerRedilValidator = registerRedilValidator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RedilListDto>>>> GetRediles()
        {
            var rediles = await _redilService.GetRediles();

            return Ok(new ApiResponse<IEnumerable<RedilListDto>>
            {
                Success = true,
                Message = "Rediles obtenidos",
                Data = rediles
            });
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<RedilDto>>> RegisterRedil([FromBody]RegisterRedilDto registerRedilDto)
        {
            var validationResult = await _registerRedilValidator.ValidateAsync(registerRedilDto);

            if(!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<RedilDto>
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

            var registerResult = await _redilService.RegisterRedil(registerRedilDto);

            if(!registerResult.Success || registerResult.Data == null)
            {
                return BadRequest(new ApiResponse<RedilDto>
                {
                    Success = false,
                    Message = registerResult.ErrorMessage ?? "Error al registrar el redil"
                });
            }

            return Ok(new ApiResponse<RedilDto>
            {
                Success = true,
                Message = "Redil registrado",
                Data = registerResult.Data
            });
        }
    }
}
