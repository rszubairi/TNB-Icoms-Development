using FluentValidation;
using TnbIcoms.Application.Users.Dtos;

namespace TnbIcoms.Application.Users.Validators;

public class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserRequestDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.RoleId).GreaterThan(0);
        RuleFor(x => x.ZoneId).GreaterThan(0);
    }
}
