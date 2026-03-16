using redil_backend.Dtos.Redil;

namespace redil_backend.Repository.Redil
{
    public interface IRedilRepository<T>
    {
        Task<T?> GetRedilByName(string name);
        Task<RedilDetailsDto?> GetRedilById(int id);
        Task<T?> GetRedil(int id);
        Task Update(T redil);

        Task<int?> GetRedilIdByCode(string code);
        Task<string?> GetRedilCodeById(int id);
        Task<T?> GetRedilByCode(string code);

        Task<IEnumerable<RedilListDto>> GetAllRediles();

        Task<bool> DoesRedilExists(string code);
        Task<bool> DoesRedilExists(int id);

        Task Add(T redil);
        Task Save();
    }
}
