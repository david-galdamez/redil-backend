using FluentValidation;
using redil_backend.Dtos.Auth;

namespace redil_backend.Validators.Auth
{
    public class AuthLoginValidator : AbstractValidator<AuthLoginDto>
    {
        public AuthLoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
        }
    }
}
