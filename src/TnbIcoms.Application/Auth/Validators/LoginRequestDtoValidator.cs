using FluentValidation;
using TnbIcoms.Application.Auth.Dtos;

namespace TnbIcoms.Application.Auth.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.StaffIdOrEmail).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty();
    }
}
