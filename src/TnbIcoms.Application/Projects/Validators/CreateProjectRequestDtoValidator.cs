using FluentValidation;
using TnbIcoms.Application.Projects.Dtos;

namespace TnbIcoms.Application.Projects.Validators;

public class CreateProjectRequestDtoValidator : AbstractValidator<CreateProjectRequestDto>
{
    public CreateProjectRequestDtoValidator()
    {
        RuleFor(x => x.TpCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProjectSuffix).NotEmpty().MaximumLength(150);
    }
}
