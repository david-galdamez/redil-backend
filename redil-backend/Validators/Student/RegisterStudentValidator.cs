using FluentValidation;
using redil_backend.Dtos.Student;

namespace redil_backend.Validators.Student
{
    public class RegisterStudentValidator : AbstractValidator<RegisterStudentDto>
    {
        public RegisterStudentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MaximumLength(100).WithMessage("El nombre no puede ser mayor a 100 caracteres");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electronico es requerido")
                .EmailAddress().WithMessage("El correo electronico no es valido");
            RuleFor(x => x.IsServer)
                .NotEmpty().WithMessage("El campo servidor es requerido");
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("El Id del grupo es requerido")
                .GreaterThan(0).WithMessage("El Id tiene que ser valido");
        }
    }
}
