using redil_backend.Dtos.Student;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.Groups;
using redil_backend.Repository.Redil;
using redil_backend.Repository.StudentRediles;
using redil_backend.Repository.Students;

namespace redil_backend.Services.Students
{
    public class StudentService : IStudentService<ServiceResult<int>, RegisterStudentDto>
    {
        private IStudentRepository<Student> _studentsRepository; 
        private IStudentRedilRepository<StudentRedil> _studentRedilRepository;
        private IGroupRepository<Group> _groupRepository;
        private IRedilRepository<Redile> _redilRepository;

        public StudentService(
            IStudentRepository<Student> studentsRepository,
            IStudentRedilRepository<StudentRedil> studentRedilRepository,
            IGroupRepository<Group> groupRepository,
            IRedilRepository<Redile> redilRepository)
        {
            _studentsRepository = studentsRepository;
            _studentRedilRepository = studentRedilRepository;
            _groupRepository = groupRepository;
            _redilRepository = redilRepository;
        }

        public async Task<ServiceResult<IEnumerable<StudentListDto>>> GetStudentByRedil(int id)
        {
            var redilExists = await _redilRepository.DoesRedilExists(id);
            if(!redilExists)
            {
                return ServiceResult<IEnumerable<StudentListDto>>.Fail("El redil no existe.");
            }

            var students = await _studentsRepository.GetStudentsByRedilId(id);

            return ServiceResult<IEnumerable<StudentListDto>>.Ok(students);
        }

        public async Task<ServiceResult<int>> RegisterStudent(RegisterStudentDto registerStudentDto, string code)
        {
            var redilId = await _redilRepository.GetRedilIdByCode(code);
            if(redilId == null)
            {
                return ServiceResult<int>.Fail("El codigo del redil no existe.");
            }

            var validateGroup = await _groupRepository.ValidateGroup(registerStudentDto.GroupId);
            if(!validateGroup)
            {
                return ServiceResult<int>.Fail("El grupo no existe.");
            }

            var student = await _studentsRepository.GetStudentByEmail(registerStudentDto.Email);
            if (student == null)
            {
                var newStudent = registerStudentDto.ToStudentModel();

                await _studentsRepository.Add(newStudent);
                await _studentsRepository.Save();

                await _studentRedilRepository.Add(new StudentRedil
                {
                    StudentId = newStudent.Id,
                    RedilId = redilId.Value,
                });
                await _studentRedilRepository.Save();

                return ServiceResult<int>.Ok(newStudent.Id);
            }

            student.Name = registerStudentDto.Name;
            student.IsServer = registerStudentDto.IsServer;
            student.GroupId = registerStudentDto.GroupId;
            student.UpdatedAt = DateTime.UtcNow;

            await _studentsRepository.Update(student);
            await _studentsRepository.Save();

            var relation = await _studentRedilRepository.GetRelation(student.Id, redilId.Value);
            if(relation != null)
            {
                relation.Active = true;
                relation.JoinedAt = DateTime.UtcNow;

                await _studentRedilRepository.Update(relation);
                await _studentRedilRepository.Save();

                return ServiceResult<int>.Ok(student.Id);
            }

            var activeRelation = await _studentRedilRepository.GetActiveRelation(student.Id);
            if (activeRelation != null)
            {
                activeRelation.Active = false;
                await _studentRedilRepository.Update(activeRelation);
            }

            await _studentRedilRepository.Add(new StudentRedil
            {
                StudentId = student.Id,
                RedilId = redilId.Value
            });
            await _studentRedilRepository.Save();

            return ServiceResult<int>.Ok(student.Id);
        }
    }
}
