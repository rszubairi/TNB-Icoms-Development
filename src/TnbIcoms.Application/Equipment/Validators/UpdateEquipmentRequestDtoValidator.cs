using FluentValidation;
using TnbIcoms.Application.Equipment.Dtos;

namespace TnbIcoms.Application.Equipment.Validators;

public class UpdateEquipmentRequestDtoValidator : AbstractValidator<UpdateEquipmentRequestDto>
{
    public UpdateEquipmentRequestDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OffPointRemark).MaximumLength(1000);
    }
}
