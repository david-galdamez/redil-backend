using redil_backend.Dtos.Redil;

namespace redil_backend.Repository.Redil
{
    public interface IRedilRepository<T>
    {
        Task<T?> GetRedilByName(string name);
        Task<T?> GetRedilById(int id);

        Task<IEnumerable<RedilListDto>> GetAllRediles();

        Task Add(T redil);
        Task Save();
    }
}
