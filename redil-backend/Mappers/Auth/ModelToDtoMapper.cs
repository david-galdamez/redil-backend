using redil_backend.Dtos.Auth;
using redil_backend.Models;

namespace redil_backend.Mappers.Auth
{
    public static class ModelToDtoMapper
    {
        public static UserDto ToUserDto(this users user)
        {
            return new UserDto(user.id, user.name, user.email);
        }
    }
}
