using FluentValidation;
using TnbIcoms.Application.Equipment.Dtos;

namespace TnbIcoms.Application.Equipment.Validators;

public class CreateEquipmentRequestDtoValidator : AbstractValidator<CreateEquipmentRequestDto>
{
    public CreateEquipmentRequestDtoValidator()
    {
        RuleFor(x => x.StationId).GreaterThan(0);
        RuleFor(x => x.VoltageLevelId).GreaterThan(0);
        RuleFor(x => x.EquipmentTypeId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OffPointRemark).MaximumLength(1000);
    }
}
