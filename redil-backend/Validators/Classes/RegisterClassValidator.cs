using FluentValidation;
using redil_backend.Dtos.Classes;

namespace redil_backend.Validators.Classes
{
    public class RegisterClassValidator : AbstractValidator<RegisterClassDto>
    {
        public RegisterClassValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("La fecha es obligatoria.")
                .Must(date => date > DateTime.MinValue).WithMessage("La fecha no es válida.")
                .Must(date => date >= DateTime.Now).WithMessage("La fecha no puede ser pasada."); 
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres.");
        }
    }
}
