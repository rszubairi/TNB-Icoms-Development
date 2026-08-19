using FluentValidation;
using TnbIcoms.Application.Organisations.Dtos;

namespace TnbIcoms.Application.Organisations.Validators;

public class CreateOrganisationRequestDtoValidator : AbstractValidator<CreateOrganisationRequestDto>
{
    public CreateOrganisationRequestDtoValidator()
    {
        RuleFor(x => x.OrganisationName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OrganisationCode).NotEmpty().MaximumLength(30);
        RuleFor(x => x.ZoneId).GreaterThan(0);
    }
}
