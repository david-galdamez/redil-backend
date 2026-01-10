using redil_backend.Dtos.Redil;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.Redil;

namespace redil_backend.Services.Redil
{
    public class RedilService : IRedilService<ServiceResult<RedilDto>, RegisterRedilDto>
    {
        private IRedilRepository<rediles> _redilRepository;
        public RedilService(IRedilRepository<rediles> redilRepository)
        {
            _redilRepository = redilRepository;
        }

        public async Task<IEnumerable<RedilListDto>> GetRediles()
        {
            return await _redilRepository.GetAllRediles();
        }

        public async Task<ServiceResult<RedilDto>> RegisterRedil(RegisterRedilDto registerRedilDto)
        {

            var redil = registerRedilDto.ToRedilModel();

            await _redilRepository.Add(redil);
            await _redilRepository.Save();

            var redilDto = redil.ToRedilDto();

            return ServiceResult<RedilDto>.Ok(redilDto);
        }
    }
}
