using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("register/{code}")]
        public async Task<ActionResult<ApiResponse<int>>> RegisterStudent([FromRoute]string code, [FromBody]RegisterStudentDto registerStudentDto)
        {
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

            return Ok();
        }
    }
}
