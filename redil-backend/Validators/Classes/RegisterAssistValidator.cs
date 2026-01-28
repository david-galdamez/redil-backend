using FluentValidation;
using redil_backend.Dtos.Classes;

namespace redil_backend.Validators.Classes
{
    public class RegisterAssistValidator : AbstractValidator<RegisterAttendanceDto>
    {
        public RegisterAssistValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.");
            RuleFor(x => x.Attended).NotNull().WithMessage("El campo 'Attended' es obligatorio.");
        }
    }
}
