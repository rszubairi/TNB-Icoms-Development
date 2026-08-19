using FluentValidation;
using TnbIcoms.Application.Roles.Dtos;

namespace TnbIcoms.Application.Roles.Validators;

public class UpdateRoleRequestDtoValidator : AbstractValidator<UpdateRoleRequestDto>
{
    public UpdateRoleRequestDtoValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(50);
    }
}
