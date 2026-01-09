using redil_backend.Domain.Enums;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Redil;
using redil_backend.Models;

namespace redil_backend.Mappers.Auth
{
    public static class DtoToModelMapper
    {
        public static users ToUserModel(this AuthRegisterDto authRegisterDto, UserRole role)
        {
            return new users
            {
                name = authRegisterDto.Name,
                email = authRegisterDto.Email,
                role_id = (int)role,
            };
        }

        public static rediles ToRedilModel(this RegisterRedilDto registerRedilDto)
        {
            return new rediles
            {
                name = registerRedilDto.Name,
                description = registerRedilDto.Description,
            };
        }
    }
}
