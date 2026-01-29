using FluentValidation;
using redil_backend.Dtos.Teacher;

namespace redil_backend.Validators.Teacher
{
    public class UpdateTeacherValidator : AbstractValidator<UpdateTeacherDto>
    {
        public UpdateTeacherValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico no puede estar vacío.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.");

            RuleFor(x => x.RedilId)
                .GreaterThan(0).WithMessage("El Id debe ser un numero valido");

            RuleFor(x => x.IsActive).NotNull().WithMessage("El estado de actividad no puede estar vacío.");
        }
    }
}
