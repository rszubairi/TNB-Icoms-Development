using FluentValidation;
using TnbIcoms.Application.VoltageLevels.Dtos;

namespace TnbIcoms.Application.VoltageLevels.Validators;

public class UpdateVoltageLevelRequestDtoValidator : AbstractValidator<UpdateVoltageLevelRequestDto>
{
    public UpdateVoltageLevelRequestDtoValidator()
    {
        RuleFor(x => x.LevelName).NotEmpty().MaximumLength(30);
    }
}
