using FluentValidation;
using redil_backend.Dtos.Redil;

namespace redil_backend.Validators.Redil
{
    public class RegisterRedilValidator : AbstractValidator<RegisterRedilDto>
    {
        public RegisterRedilValidator() 
        { 
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("El nombre del redil es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre del redil no puede exceder los 100 caracteres.");

            RuleFor(r => r.Description)
                .MaximumLength(255).WithMessage("La descripción del redil no puede exceder los 255 caracteres.");
        }
    }
}
