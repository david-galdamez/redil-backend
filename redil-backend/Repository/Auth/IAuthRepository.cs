using redil_backend.Models;

namespace redil_backend.Repository.Auth
{
    public interface IAuthRepository<TEntity>
    {
        Task<TEntity?> GetUserByEmail(string email);
        Task Add(TEntity entity);
        Task Save();
    }
}
