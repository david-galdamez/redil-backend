using redil_backend.Dtos.Redil;

namespace redil_backend.Services.Redil
{
    public interface IRedilService<T, Tr>
    {
        Task<IEnumerable<RedilListDto>> GetRediles();
        Task<T> RegisterRedil(Tr registerRedilDto);
        Task<ServiceResult<string>> GetRedilCode(int redilId);
        Task<T> GetRedilByCode(string code);
        Task<ServiceResult<RedilDetailsDto>> GetRedilById(int id);
        Task<bool> RedilExists(int id);
        Task<ServiceResult<RedilDetailsDto>> UpdateRedil(int id, Tr updateRedilDto);
    }
}
