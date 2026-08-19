using FluentValidation;
using TnbIcoms.Application.EquipmentTypes.Dtos;

namespace TnbIcoms.Application.EquipmentTypes.Validators;

public class CreateEquipmentTypeRequestDtoValidator : AbstractValidator<CreateEquipmentTypeRequestDto>
{
    public CreateEquipmentTypeRequestDtoValidator()
    {
        RuleFor(x => x.TypeName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.VoltageLevelId).GreaterThan(0);
    }
}
