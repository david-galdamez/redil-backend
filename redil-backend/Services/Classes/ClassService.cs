using redil_backend.Dtos.Classes;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.Classes;

namespace redil_backend.Services.Classes
{
    public class ClassService : IClassService<ServiceResult<ClassDto>, RegisterClassDto>
    {
        private IClassRepository<classes> _classRepository;

        public ClassService(IClassRepository<classes> classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task<ServiceResult<ClassDto>> RegisterClass(RegisterClassDto registerClassDto, int redilId, int teacherId)
        {
            var classModel = registerClassDto.ToClassModel(redilId, teacherId);

            await _classRepository.Add(classModel);
            await _classRepository.Save();

            var classDto = classModel.ToClassDto();

            return ServiceResult<ClassDto>.Ok(classDto);
        }
    }
}
