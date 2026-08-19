using FluentValidation;
using TnbIcoms.Application.AuthorisationPersonnel.Dtos;

namespace TnbIcoms.Application.AuthorisationPersonnel.Validators;

public class SaveAuthorisationPersonnelRequestDtoValidator : AbstractValidator<SaveAuthorisationPersonnelRequestDto>
{
    public SaveAuthorisationPersonnelRequestDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.ZoneId).GreaterThan(0);
    }
}
