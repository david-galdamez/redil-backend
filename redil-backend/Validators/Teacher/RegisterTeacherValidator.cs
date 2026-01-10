using FluentValidation;
using redil_backend.Dtos.Teacher;

namespace redil_backend.Validators.Teacher
{
    public class RegisterTeacherValidator : AbstractValidator<RegisterTeacherDto>
    {
        public RegisterTeacherValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MaximumLength(100).WithMessage("El nombre no puede ser mayor a 100 caracteres");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es requerido")
                .EmailAddress().WithMessage("El correo electrónico no es válido")
                .MaximumLength(100).WithMessage("El correo electrónico no puede ser mayor a 100 caracteres");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres");
            RuleFor(x => x.RedilId)
                .NotEmpty().WithMessage("El rol es obligatorio")
                .GreaterThan(0).WithMessage("El rol debe ser un valor positivo");
        }
    }
}
