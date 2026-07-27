using FluentValidation;
using redil_backend.Dtos.Classes;

namespace redil_backend.Validators.Classes
{
    public class RegisterAssistValidator : AbstractValidator<RegisterAttendanceDto>
    {
        public RegisterAssistValidator()
        {
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El número de teléfono es obligatorio.")
                .MaximumLength(20).WithMessage("El número de teléfono no puede exceder 20 caracteres.");

            RuleFor(x => x.Attended)
                .NotNull().WithMessage("El campo 'Attended' es obligatorio.");
        }
    }
}
