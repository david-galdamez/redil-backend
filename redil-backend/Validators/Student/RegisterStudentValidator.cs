using FluentValidation;
using redil_backend.Dtos.Student;

namespace redil_backend.Validators.Student
{
    public class RegisterStudentValidator : AbstractValidator<RegisterStudentDto>
    {
        public RegisterStudentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no puede ser mayor a 100 caracteres.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El número de teléfono es requerido.")
                .MaximumLength(20).WithMessage("El número de teléfono no puede exceder 20 caracteres.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El correo electrónico no es válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.IsServer)
                .NotNull().WithMessage("El campo servidor es requerido.");

            RuleFor(x => x.GroupId)
                .GreaterThan(0).WithMessage("El Id del grupo debe ser válido.");
        }
    }
}
