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

        public async Task<ServiceResult<RedilDto>> GetRedilByCode(string code)
        {
            var redil = await _redilRepository.GetRedilByCode(code);

            if(redil == null)
            {
                return ServiceResult<RedilDto>.Fail("Redil no encontrado.");
            }

            var redilDto = redil.ToRedilDto();
            return ServiceResult<RedilDto>.Ok(redilDto);
        }

        public async Task<ServiceResult<string>> GetRedilCode(int redilId)
        {
            var redilCode = await _redilRepository.GetRedilCodeById(redilId);
            if(redilCode == null)
            {
                return ServiceResult<string>.Fail("Redil no encontrado.");
            }

            return ServiceResult<string>.Ok(redilCode);
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
