using FluentValidation;
using redil_backend.Dtos.Classes;

namespace redil_backend.Validators.Classes
{
    public class StatClassRequestValidator : AbstractValidator<ClassStatsRequestDto>
    {
        public StatClassRequestValidator()
        {
            RuleFor(x => x.FromDate)
                .NotEmpty().WithMessage("La fecha de inicio es obligatoria.")
                .Must(date => date > DateTime.MinValue).WithMessage("La fecha de inicio no es válida.");
            RuleFor(x => x.ToDate)
                .NotEmpty().WithMessage("La fecha de fin es obligatoria.")
                .Must(date => date > DateTime.MinValue).WithMessage("La fecha final no es válida.");
            RuleFor(x => x)
                .Must(dates => dates.ToDate >= dates.FromDate)
                .WithMessage("La fecha final debe ser mayor o igual a la fecha de inicio.");
        }
    }
}
