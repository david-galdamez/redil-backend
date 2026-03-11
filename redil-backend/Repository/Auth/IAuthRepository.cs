using redil_backend.Dtos.Teacher;
using redil_backend.Models;

namespace redil_backend.Repository.Auth
{
    public interface IAuthRepository<TEntity>
    {
        Task<TEntity?> GetUserByEmail(string email);
        Task<IEnumerable<TeacherListDto>> GetAllTeachers(int page);
        Task<TEntity> GetTeacher(int teacherId);
        Task<TEntity?> GetUserById(int id);
        Task<bool> TeacherExists(string email);
        Task<bool> TeacherExists(int teacherId);
        Task Add(TEntity entity);
        Task Update(TEntity entity);
        Task Save();
    }
}
