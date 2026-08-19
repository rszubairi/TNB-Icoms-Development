using FluentValidation;
using TnbIcoms.Application.DropdownValues.Dtos;

namespace TnbIcoms.Application.DropdownValues.Validators;

public class UpdateDropdownValueRequestDtoValidator : AbstractValidator<UpdateDropdownValueRequestDto>
{
    public UpdateDropdownValueRequestDtoValidator()
    {
        RuleFor(x => x.ValueLabel).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ParentCode).MaximumLength(50);
    }
}
