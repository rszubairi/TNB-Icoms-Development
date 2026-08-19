using FluentValidation;
using TnbIcoms.Application.ChangeRequests.Dtos;

namespace TnbIcoms.Application.ChangeRequests.Validators;

public class CreateChangeRequestBatchDtoValidator : AbstractValidator<CreateChangeRequestBatchDto>
{
    public CreateChangeRequestBatchDtoValidator()
    {
        RuleFor(x => x.OutageId).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty();
    }
}
