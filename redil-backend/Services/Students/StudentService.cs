using redil_backend.Dtos.Student;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.Redil;
using redil_backend.Repository.Students;

namespace redil_backend.Services.Students
{
    public class StudentService : IStudentService<ServiceResult<int>, RegisterStudentDto>
    {
        private IStudentRepository<Student> _studentsRepository; 
        private IRedilRepository<Redile> _redilRepository;

        public StudentService(
            IStudentRepository<Student> studentsRepository,
            IRedilRepository<Redile> redilRepository)
        {
            _studentsRepository = studentsRepository;
            _redilRepository = redilRepository;
        }
        public async Task<ServiceResult<int>> RegisterStudent(RegisterStudentDto registerStudentDto, string code)
        {
            var redilId = await _redilRepository.GetRedilIdByCode(code);
            if(redilId == null)
            {
                return ServiceResult<int>.Fail("El codigo del redil no existe.");
            }

            var isEmailTaken = await _studentsRepository.ValidateStudent(registerStudentDto.Email);
            if (isEmailTaken)
            {
                return ServiceResult<int>.Fail("El correo ya está registrado.");
            }

            var newStudent = registerStudentDto.ToStudentModel();

            await _studentsRepository.Add(newStudent);
            await _studentsRepository.Save();

            return ServiceResult<int>.Ok(newStudent.Id);
        }
    }
}
