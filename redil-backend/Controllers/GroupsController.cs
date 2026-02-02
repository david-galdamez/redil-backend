using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using redil_backend.Domain.Enums;
using redil_backend.Dtos.Groups;
using redil_backend.Dtos.Responses;
using redil_backend.Services;
using redil_backend.Services.Groups;

namespace redil_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private IGroupService<ServiceResult<int>> _groupService;
        private IValidator<RegisterGroupDto> _registerGroupValidator;
        public GroupsController(
            IGroupService<ServiceResult<int>> groupService,
            IValidator<RegisterGroupDto> registerGroupValidator)
        {
            _groupService = groupService;
            _registerGroupValidator = registerGroupValidator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ICollection<GroupsListDto>>>> GetGroups()
        {
            var groups = await _groupService.GetAllGroups();

            return Ok(new ApiResponse<ICollection<GroupsListDto>>
            {
                Success = true,
                Data = groups.Data
            });
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> CreateGroup([FromBody] RegisterGroupDto registerGroupDto)
        {
            var validationResult = await _registerGroupValidator.ValidateAsync(registerGroupDto);
            if(!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = "Error de validacion",
                    Errors = validationResult.Errors
                        .Select(e => new ApiError
                        {
                            Field = e.PropertyName,
                            Message = e.ErrorMessage
                        })
                        .ToList()
                });
            }

            var result = await _groupService.AddGroup(registerGroupDto);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = result.ErrorMessage
                });
            }
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Grupo registrado con exito",
            });
        }
    }
}
