using FluentValidation;
using TnbIcoms.Application.TransmissionLines.Dtos;

namespace TnbIcoms.Application.TransmissionLines.Validators;

public class TransmissionLineRequestDtoValidator : AbstractValidator<TransmissionLineRequestDto>
{
    public TransmissionLineRequestDtoValidator()
    {
        RuleFor(x => x.VoltageLevelId).GreaterThan(0);
        RuleFor(x => x.EquipmentTypeId).GreaterThan(0);
        RuleFor(x => x.NamingInteger).GreaterThan(0);
        RuleFor(x => x.LineNumber).GreaterThan(0);
        RuleFor(x => x.StationIdsInOrder).Must(s => s.Count is >= 2 and <= 4)
            .WithMessage("A line needs between 2 and 4 stations.");
    }
}
