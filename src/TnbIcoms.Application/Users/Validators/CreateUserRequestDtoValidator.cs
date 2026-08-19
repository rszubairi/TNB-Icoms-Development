using FluentValidation;
using TnbIcoms.Application.Users.Dtos;

namespace TnbIcoms.Application.Users.Validators;

public class CreateUserRequestDtoValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserRequestDtoValidator()
    {
        RuleFor(x => x.TnbId).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.RoleId).GreaterThan(0);
        RuleFor(x => x.ZoneId).GreaterThan(0);
    }
}
