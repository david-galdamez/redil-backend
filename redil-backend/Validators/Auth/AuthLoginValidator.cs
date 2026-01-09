using FluentValidation;
using redil_backend.Dtos.Auth;

namespace redil_backend.Validators.Auth
{
    public class AuthLoginValidator : AbstractValidator<AuthLoginDto>
    {
        public AuthLoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("El correo es requerido.")
                .EmailAddress().WithMessage("Formato de correo incorrecto.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(8).WithMessage("Contraseña debe de ser de al menos 8 caracteres");
        }
    }
}
