using Microsoft.AspNetCore.Identity;
using redil_backend.Domain.Enums;
using redil_backend.Dtos.Teacher;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.Auth;

namespace redil_backend.Services.Teacher
{
    public class TeacherService : ITeacherService<ServiceResult<TeacherDto>, RegisterTeacherDto>
    {

        private IAuthRepository<users> _authRepository;
        private IPasswordHasher<users> _passwordHasher;

        public TeacherService(IAuthRepository<users> autoRepository, IPasswordHasher<users> passwordHasher)
        {
            _authRepository = autoRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResult<TeacherDto>> RegisterTeacher(RegisterTeacherDto registerTeacherDto)
        {
            var teacher = registerTeacherDto.ToTeacherModel(UserRole.Maestro);

            var hashedPassword = _passwordHasher.HashPassword(teacher, registerTeacherDto.Password);

            teacher.password = hashedPassword;

            await _authRepository.Add(teacher);
            await _authRepository.Save();

            return ServiceResult<TeacherDto>.Ok(teacher.ToTeacherDto());
        }

        public async Task<bool> ValidateTeacher(string email)
        {
            var teacher = await _authRepository.GetUserByEmail(email);

            return teacher == null;
        } 
    }
}
