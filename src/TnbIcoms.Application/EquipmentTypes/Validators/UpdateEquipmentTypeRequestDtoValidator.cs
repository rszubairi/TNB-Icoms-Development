using FluentValidation;
using TnbIcoms.Application.EquipmentTypes.Dtos;

namespace TnbIcoms.Application.EquipmentTypes.Validators;

public class UpdateEquipmentTypeRequestDtoValidator : AbstractValidator<UpdateEquipmentTypeRequestDto>
{
    public UpdateEquipmentTypeRequestDtoValidator()
    {
        RuleFor(x => x.TypeName).NotEmpty().MaximumLength(100);
    }
}
