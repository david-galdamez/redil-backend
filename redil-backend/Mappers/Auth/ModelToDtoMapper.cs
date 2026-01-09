using redil_backend.Domain.Enums;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Redil;
using redil_backend.Models;

namespace redil_backend.Mappers.Auth
{
    public static class ModelToDtoMapper
    {
        public static UserDto ToUserDto(this users user)
        {
            return new UserDto(user.id, user.name, user.email, (UserRole)user.role_id);
        }

        public static RedilDto ToRedilDto(this rediles redil)
        {
            return new RedilDto(redil.id, redil.name, redil.description);
        }
    }
}
