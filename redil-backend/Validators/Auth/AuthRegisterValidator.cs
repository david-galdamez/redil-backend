using FluentValidation;
using redil_backend.Dtos.Auth;

namespace redil_backend.Validators.Auth
{
    public class AuthRegisterValidator : AbstractValidator<AuthRegisterDto>
    {
        public AuthRegisterValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("El correo es requerido")
                .EmailAddress().WithMessage("Formato de correo incorrecto");
            RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres");
            RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(8).WithMessage("Contraseña debe de ser de al menos 8 caracteres");
        }
    }
}
