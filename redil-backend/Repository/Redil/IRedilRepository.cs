using redil_backend.Dtos.Redil;

namespace redil_backend.Repository.Redil
{
    public interface IRedilRepository<T>
    {
        Task<T?> GetRedilByName(string name);
        Task<T?> GetRedilById(int id);

        Task<int?> GetRedilIdByCode(string code);

        Task<IEnumerable<RedilListDto>> GetAllRediles();

        Task<bool> DoesRedilExists(string code);

        Task Add(T redil);
        Task Save();
    }
}
