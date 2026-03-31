using Microsoft.EntityFrameworkCore;
using redil_backend.Dtos;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Redil;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.ClassDetails;
using redil_backend.Repository.Classes;
using redil_backend.Repository.Redil;
using redil_backend.Repository.StudentRediles;
using redil_backend.Repository.Students;
using redil_backend.Utils;

namespace redil_backend.Services.Classes
{
    public class ClassService : IClassService<ServiceResult<ClassDto>, RegisterClassDto>
    {
        private IClassRepository<Class> _classRepository;
        private IStudentRedilRepository<StudentRedil> _studentRedilRepository;
        private IClassDetailsRepository<ClassDetail> _classDetailsRepository;
        private IStudentRepository<Student> _studentRepository;
        private IRedilRepository<Redile> _redilRepository;

        public ClassService(
            IClassRepository<Class> classRepository,
            IStudentRedilRepository<StudentRedil> studentRedilRepository,
            IClassDetailsRepository<ClassDetail> classDetailsRepository,
            IStudentRepository<Student> studentRepository,
            IRedilRepository<Redile> redilRepository)
        {
            _studentRedilRepository = studentRedilRepository;
            _classRepository = classRepository;
            _classDetailsRepository = classDetailsRepository;
            _studentRepository = studentRepository;
            _redilRepository = redilRepository;
        }

        public async Task<bool> ClassExists(int classId)
        {
            return await _classRepository.Exists(classId);
        }

        public async Task<bool> ClassExists(string attendanceToken)
        {
            return await _classRepository.Exists(attendanceToken);
        }

        public async Task<ServiceResult<AssistStatusDto>> GetAssistStatus(string attendanceToken)
        {
            var validAssist = await ValidateAssistToken(attendanceToken);
            if(!validAssist)
            {
                return ServiceResult<AssistStatusDto>.Fail("Token de asistencia inválido o expirado.");
            }

            var classModel = await _classRepository.GetByAttendanceToken(attendanceToken);
            if(classModel == null)
            {
                return ServiceResult<AssistStatusDto>.Fail("Clase no encontrada.");
            }

            var redil = await _redilRepository.GetRedilById(classModel.RedilId);
            if(redil == null)
            {
                return ServiceResult<AssistStatusDto>.Fail("Redil no encontrado.");
            }

            var assistStatus = new AssistStatusDto(redil.Name, classModel.ClassDescription, classModel.ClassDate);
            return ServiceResult<AssistStatusDto>.Ok(assistStatus);
        }

        public async Task<ServiceResult<ClassDetailsDto>> GetClassDetail(int classId)
        {
            var classDetail = await _classRepository.GetById(classId);

            if(classDetail == null)
            {
                return ServiceResult<ClassDetailsDto>.Fail("Clase no encontrada.");
            }

            return ServiceResult<ClassDetailsDto>.Ok(classDetail.ToClassDetailsDto());
        }

        public async Task<ServiceResult<PaginatedResponse<ClassListDto>>> GetClasses(int teacherId, int page)
        {
            var classes = await _classRepository.GetClasses(teacherId, page);

            return ServiceResult<PaginatedResponse<ClassListDto>>.Ok(classes);
        }

        public async Task<ServiceResult<ICollection<RedilClassStatDto>>> GetRedilStats(int? redilId, ClassStatsRequestDto classStatsRequest)
        {
            var details = await _classDetailsRepository.GetClassDetailsForStats(redilId, classStatsRequest.FromDate, classStatsRequest.ToDate, classStatsRequest.GroupId);
            if (!details.Any())
            {
                return ServiceResult<ICollection<RedilClassStatDto>>.Ok(new List<RedilClassStatDto>());
            }

            var totalClasses = details.Select(d => d.Class.ClassDate.Date).Distinct().Count();

            var stats = details.GroupBy(d => new
            {
                d.Student,
                d.Class.Redil.Name
            })
                .Select(g =>
                {
                    var attended = g.Count(d => d.Attendance);

                    var porcentage = totalClasses == 0 ? 0 : (float)attended / totalClasses * 100;

                    return new RedilClassStatDto(g.Key.Student.Name, g.Key.Student.Group.Name, g.Key.Name, g.Key.Student.IsServer, porcentage);
                }).ToList();

            return ServiceResult<ICollection<RedilClassStatDto>>.Ok(stats);
        }

        public async Task<ServiceResult<string>> PassAssist(int classId)
        {
            var classModel = await _classRepository.GetById(classId);
            if(classModel == null)
            {
                return ServiceResult<string>.Fail("Clase no encontrada.");
            }

            // Si se intenta generar otro token, se reutiliza el token existente si no ha expirado
            if(classModel.AttendanceToken != null && classModel.ExpiresAt > DateTime.UtcNow)
            {
                return ServiceResult<string>.Ok(classModel.AttendanceToken);
            }

            bool saved = false;
            while(!saved)
            {
                classModel.AttendanceToken = AttendanceTokenGenerator.Generate();
                classModel.ExpiresAt = DateTime.UtcNow.AddHours(1);

                try
                {
                    await _classRepository.Update(classModel);
                    await _classRepository.Save();
                    saved = true;
                }
                catch (DbUpdateException)
                {
                    // Si hay una colisión de token, generamos uno nuevo y lo intentamos de nuevo
                    continue;
                }
            }

            return ServiceResult<string>.Ok(classModel.AttendanceToken!);
        }

        public async Task<ServiceResult<ClassDto>> RegisterAssist(string attendanceToken, RegisterAttendanceDto registerAttendanceDto)
        {
            var validAssist = await ValidateAssistToken(attendanceToken);
            if(!validAssist)
            {
                return ServiceResult<ClassDto>.Fail("Token de asistencia inválido o expirado.");
            }

            var classModel = await _classRepository.GetByAttendanceToken(attendanceToken);
            if(classModel == null)
            {
                return ServiceResult<ClassDto>.Fail("Clase no encontrada.");
            }

            var studentModel = await _studentRepository.GetStudentByEmail(registerAttendanceDto.Email, classModel.RedilId);
            if(studentModel == null)
            {
                return ServiceResult<ClassDto>.Fail("Correo no registrado en el redil.");
            }

            var classDetail = await _classDetailsRepository.GetClassDetail(classModel.Id, studentModel.Id);
            if(classDetail == null)
            {
                return ServiceResult<ClassDto>.Fail("El estudiante no está inscrito en esta clase.");
            }

            if (classDetail.Attendance)
            {
                return ServiceResult<ClassDto>.Fail("Asistencia ya registrada para este estudiante.");
            }

            classDetail.Attendance = true;

            await _classDetailsRepository.Update(classDetail);
            await _classDetailsRepository.Save();

            return ServiceResult<ClassDto>.Ok(classModel.ToClassDto());
        }

        public async Task<ServiceResult<ClassDto>> RegisterClass(RegisterClassDto registerClassDto, int redilId, int teacherId)
        {
            var classModel = registerClassDto.ToClassModel(redilId, teacherId);

            await _classRepository.Add(classModel);
            await _classRepository.Save();

            var students = await _studentRedilRepository.GetStudents(redilId);
            foreach(var student in students)
            {
                await _classDetailsRepository.Add(new ClassDetail
                {
                    ClassId = classModel.Id,
                    StudentId = student.StudentId,
                    Attendance = false
                });
            }

            await _classDetailsRepository.Save();

            var classDto = classModel.ToClassDto();
            return ServiceResult<ClassDto>.Ok(classDto);
        }

        public async Task<bool> ValidateAssistToken(string attendanceToken)
        {
            var classModel = await _classRepository.GetByAttendanceToken(attendanceToken);
            if(classModel == null)
            {
                return false;
            }

            return classModel.AttendanceToken != null && classModel.ExpiresAt > DateTime.UtcNow;
        }
    }
}
