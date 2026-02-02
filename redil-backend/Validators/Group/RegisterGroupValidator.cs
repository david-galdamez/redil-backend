using FluentValidation;
using redil_backend.Dtos.Groups;

namespace redil_backend.Validators.Group
{
    public class RegisterGroupValidator : AbstractValidator<RegisterGroupDto>
    {
        public RegisterGroupValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del grupo es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre del grupo no puede exceder los 100 caracteres.");;
        }
    }
}
