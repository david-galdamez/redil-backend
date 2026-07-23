using FluentValidation;
using redil_backend.Dtos.Auth;

namespace redil_backend.Validators.Auth
{
    public class AuthUpdatePasswordValidator : AbstractValidator<UserPasswordChangeDto>
    {
        public AuthUpdatePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("La contraseña actual es requerida")
                .MinimumLength(8).WithMessage("Contraseña debe de ser de al menos 8 caracteres");
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("La nueva contraseña es requerida")
                .MinimumLength(8).WithMessage("Contraseña debe de ser de al menos 8 caracteres");
        }
    }
}
