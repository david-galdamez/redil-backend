using redil_backend.Domain.Enums;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Teacher;
using redil_backend.Models;
using System.Net.NetworkInformation;

namespace redil_backend.Mappers
{
    public static class ModelToDtoMapper
    {
        public static UserDto ToUserDto(this User user)
        {
            return new UserDto(user.Id, user.Name, user.Email, (UserRole)user.RoleId, user.RedilId);
        }

        public static TeacherDto ToTeacherDto(this User teacher)
        {
            return new TeacherDto(teacher.Name, teacher.Email, teacher.Redil?.Id ?? 0, teacher.IsActive);
        }

        public static RedilDto ToRedilDto(this Redile redil)
        {
            return new RedilDto(redil.Id, redil.Name, redil.Description);
        }

        public static ClassDto ToClassDto(this Class classes)
        {
            return new ClassDto(classes.RedilId, classes.TeacherId, classes.ClassDate, classes.ClassDescription);
        }
    }
}
