using FluentValidation;
using TnbIcoms.Application.Outages.Dtos;

namespace TnbIcoms.Application.Outages.Validators;

public class CreateOutageRequestDtoValidator : AbstractValidator<CreateOutageRequestDto>
{
    public CreateOutageRequestDtoValidator()
    {
        RuleFor(x => x.StationId).GreaterThan(0);
        RuleFor(x => x.VoltageLevelId).GreaterThan(0);
        RuleFor(x => x.PrimaryEquipmentId).GreaterThan(0);
        RuleFor(x => x.WorkTypeCode).NotEmpty();
        RuleFor(x => x.JobTypeId).GreaterThan(0);
        RuleFor(x => x.PlannedStartAt).NotEmpty();
        RuleFor(x => x.PlannedEndAt).NotEmpty();
        RuleForEach(x => x.Pics).ChildRules(pic =>
        {
            pic.RuleFor(p => p.PicName).NotEmpty();
            pic.RuleFor(p => p.PicEmail).NotEmpty().EmailAddress();
        });
    }
}
