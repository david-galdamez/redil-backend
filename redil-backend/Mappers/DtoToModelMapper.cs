using redil_backend.Domain.Enums;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Student;
using redil_backend.Dtos.Teacher;
using redil_backend.Models;

namespace redil_backend.Mappers
{
    public static class DtoToModelMapper
    {
        public static User ToUserModel(this AuthRegisterDto authRegisterDto, UserRole role)
        {
            return new User
            {
                Name = authRegisterDto.Name,
                Email = authRegisterDto.Email,
                RoleId = (int)role,
            };
        }

        public static User ToTeacherModel(this RegisterTeacherDto registerTeacherDto, UserRole role)
        {
            return new User
            {
                Name = registerTeacherDto.Name,
                Email = registerTeacherDto.Email,
                RoleId = (int)role,
                RedilId = registerTeacherDto.RedilId,
            };
        }

        public static Redile ToRedilModel(this RegisterRedilDto registerRedilDto)
        {
            return new Redile
            {
                Name = registerRedilDto.Name,
                Description = registerRedilDto.Description,
            };
        }

        public static Class ToClassModel(this RegisterClassDto registerClassDto, int RedilId, int TeacherId)
        {
            return new Class
            {
                RedilId = RedilId,
                TeacherId = TeacherId,
                ClassDate = registerClassDto.Date,
                ClassDescription = registerClassDto.Description,
            };
        }

        public static Student ToStudentModel(this RegisterStudentDto registerStudentDto)
        {
            return new Student
            {
                Name = registerStudentDto.Name,
                Email = registerStudentDto.Email,
                GroupId = registerStudentDto.GroupId,
                IsServer = registerStudentDto.IsServer,
            };
        }
    }
}
