using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using redil_backend.Dtos.Responses;
using redil_backend.Dtos.Student;
using redil_backend.Services;
using redil_backend.Services.Students;

namespace redil_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private IStudentService<ServiceResult<int>, RegisterStudentDto> _studentService;
        private IValidator<RegisterStudentDto> _registerStudentValidator;
        public StudentController(
            IStudentService<ServiceResult<int>, RegisterStudentDto> studentService,
            IValidator<RegisterStudentDto> registerStudentValidator)
        {
            _registerStudentValidator = registerStudentValidator;
            _studentService = studentService;
        }

        [Authorize]
        [HttpGet("redil/{id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<StudentListDto>>>> GetStudentsByRedil([FromRoute]int id)
        {
            if(id == 0)
            {
                return BadRequest(new ApiResponse<IEnumerable<StudentListDto>>
                {
                    Success = false,
                    Message = "Id no proporcionado."
                });
            }

            var studentResult = await _studentService.GetStudentByRedil(id);
            if(!studentResult.Success || studentResult.Data == null)
            {
                return BadRequest(new ApiResponse<IEnumerable<StudentListDto>>
                {
                    Success = false,
                    Message = studentResult.ErrorMessage
                });
            }

            return Ok(new ApiResponse<IEnumerable<StudentListDto>>
            {
                Success = true,
                Data = studentResult.Data
            });
        }

        [HttpPost("register/{code}")]
        public async Task<ActionResult<ApiResponse<int>>> RegisterStudent([FromRoute]string code, [FromBody]RegisterStudentDto registerStudentDto)
        {

            if(code.IsNullOrEmpty())
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = "Codigo no proporcionado."
                });
            }

            var validationResult = await _registerStudentValidator.ValidateAsync(registerStudentDto);
            if(!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Errors = validationResult.Errors.Select(e => new ApiError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList()
                });
            }

            var registerResult = await _studentService.RegisterStudent(registerStudentDto, code);
            if(!registerResult.Success || registerResult.Data == 0)
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = registerResult.ErrorMessage
                });
            }

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Estudiante registrado exitosamente.",
                Data = registerResult.Data
            });
        }
    }
}
