using FluentValidation;
using TnbIcoms.Application.Account.Dtos;

namespace TnbIcoms.Application.Account.Validators;

public class UpdateAccountProfileRequestDtoValidator : AbstractValidator<UpdateAccountProfileRequestDto>
{
    public UpdateAccountProfileRequestDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
    }
}
