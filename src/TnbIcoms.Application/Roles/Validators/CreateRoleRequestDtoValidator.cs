using FluentValidation;
using TnbIcoms.Application.Roles.Dtos;

namespace TnbIcoms.Application.Roles.Validators;

public class CreateRoleRequestDtoValidator : AbstractValidator<CreateRoleRequestDto>
{
    public CreateRoleRequestDtoValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.RoleCode).NotEmpty().MaximumLength(20).Matches("^[A-Z0-9_]+$")
            .WithMessage("Role code must be uppercase letters, numbers, and underscores only.");
    }
}
