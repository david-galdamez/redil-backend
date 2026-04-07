using FluentValidation;
using redil_backend.Dtos.Auth;

namespace redil_backend.Validators.Auth
{
    public class AuthUpdateProfileValidator : AbstractValidator<UserProfileUpdateDto>
    {
        public AuthUpdateProfileValidator() 
        { 
            RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}
