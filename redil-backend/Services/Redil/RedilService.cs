using redil_backend.Dtos.Redil;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.Redil;
using redil_backend.Utils;

namespace redil_backend.Services.Redil
{
    public class RedilService : IRedilService<ServiceResult<RedilDto>, RegisterRedilDto>
    {
        private IRedilRepository<Redile> _redilRepository;
        public RedilService(IRedilRepository<Redile> redilRepository)
        {
            _redilRepository = redilRepository;
        }

        public async Task<IEnumerable<RedilListDto>> GetRediles()
        {
            return await _redilRepository.GetAllRediles();
        }

        public async Task<bool> RedilExists(int id)
        {
            return await _redilRepository.DoesRedilExists(id);
        }

        public async Task<ServiceResult<RedilDto>> RegisterRedil(RegisterRedilDto registerRedilDto)
        {

            var redil = registerRedilDto.ToRedilModel();
            string code;

            do
            {
                code = RedilCodeGenerator.Generate();
            }
            while (await _redilRepository.DoesRedilExists(code));
            redil.Code = code;

            await _redilRepository.Add(redil);
            await _redilRepository.Save();

            var redilDto = redil.ToRedilDto();

            return ServiceResult<RedilDto>.Ok(redilDto);
        }
    }
}
