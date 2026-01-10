using redil_backend.Domain.Enums;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Teacher;
using redil_backend.Models;
using System.Net.NetworkInformation;

namespace redil_backend.Mappers
{
    public static class ModelToDtoMapper
    {
        public static UserDto ToUserDto(this users user)
        {
            return new UserDto(user.id, user.name, user.email, (UserRole)user.role_id);
        }

        public static TeacherDto ToTeacherDto(this users teacher)
        {
            return new TeacherDto(teacher.name, teacher.email, teacher.redil?.name ?? "", (UserRole)teacher.role_id);
        }

        public static RedilDto ToRedilDto(this rediles redil)
        {
            return new RedilDto(redil.id, redil.name, redil.description);
        }
    }
}
