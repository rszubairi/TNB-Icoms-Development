using FluentValidation;
using TnbIcoms.Application.Stations.Dtos;

namespace TnbIcoms.Application.Stations.Validators;

public class UpdateStationRequestDtoValidator : AbstractValidator<UpdateStationRequestDto>
{
    public UpdateStationRequestDtoValidator()
    {
        RuleFor(x => x.StationName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.StationAbbr).NotEmpty().MaximumLength(20);
        RuleFor(x => x.OrgId).GreaterThan(0);
    }
}
