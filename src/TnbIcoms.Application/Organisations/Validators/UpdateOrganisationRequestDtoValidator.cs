using FluentValidation;
using TnbIcoms.Application.Organisations.Dtos;

namespace TnbIcoms.Application.Organisations.Validators;

public class UpdateOrganisationRequestDtoValidator : AbstractValidator<UpdateOrganisationRequestDto>
{
    public UpdateOrganisationRequestDtoValidator()
    {
        RuleFor(x => x.OrganisationName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OrganisationCode).NotEmpty().MaximumLength(30);
    }
}
