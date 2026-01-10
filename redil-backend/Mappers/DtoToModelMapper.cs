using redil_backend.Domain.Enums;
using redil_backend.Dtos.Auth;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Redil;
using redil_backend.Dtos.Teacher;
using redil_backend.Models;

namespace redil_backend.Mappers
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

        public static users ToTeacherModel(this RegisterTeacherDto registerTeacherDto, UserRole role)
        {
            return new users
            {
                name = registerTeacherDto.Name,
                email = registerTeacherDto.Email,
                role_id = (int)role,
                redil_id = registerTeacherDto.RedilId,
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

        public static classes ToClassModel(this RegisterClassDto registerClassDto, int RedilId, int TeacherId)
        {
            return new classes
            {
                redil_id = RedilId,
                teacher_id = TeacherId,
                class_date = registerClassDto.Date,
                class_description = registerClassDto.Description,
            };
        }
    }
}
