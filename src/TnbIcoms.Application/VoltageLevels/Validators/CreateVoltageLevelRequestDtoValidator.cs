using FluentValidation;
using TnbIcoms.Application.VoltageLevels.Dtos;

namespace TnbIcoms.Application.VoltageLevels.Validators;

public class CreateVoltageLevelRequestDtoValidator : AbstractValidator<CreateVoltageLevelRequestDto>
{
    public CreateVoltageLevelRequestDtoValidator()
    {
        RuleFor(x => x.LevelName).NotEmpty().MaximumLength(30);
    }
}
