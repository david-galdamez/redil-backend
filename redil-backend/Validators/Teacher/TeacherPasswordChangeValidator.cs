using FluentValidation;
using redil_backend.Dtos.Teacher;

namespace redil_backend.Validators.Teacher
{
    public class TeacherPasswordChangeValidator : AbstractValidator<TeacherPasswordChangeDto>
    {
        public TeacherPasswordChangeValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La contraseña no puede estar vacía.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula.")
                .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula.")
                .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número.");
        }
    }
}
