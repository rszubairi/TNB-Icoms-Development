using FluentValidation;
using TnbIcoms.Application.DropdownValues.Dtos;

namespace TnbIcoms.Application.DropdownValues.Validators;

public class CreateDropdownValueRequestDtoValidator : AbstractValidator<CreateDropdownValueRequestDto>
{
    public CreateDropdownValueRequestDtoValidator()
    {
        RuleFor(x => x.CategoryCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ValueLabel).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ParentCode).MaximumLength(50);
    }
}
