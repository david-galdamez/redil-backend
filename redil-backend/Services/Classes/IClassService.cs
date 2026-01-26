using redil_backend.Dtos.Classes;

namespace redil_backend.Services.Classes
{
    public interface IClassService<T, Tr>
    {
        Task<T> RegisterClass(Tr registerClassDto, int redilId, int teacherId);
        Task<ServiceResult<ICollection<ClassListDto>>> GetClasses(int teacherId, int page);
    }
}
