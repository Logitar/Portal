using FluentValidation;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Application.Roles.Validators;

internal class RoleModificationValidator : AbstractValidator<RoleModification>
{
  public RoleModificationValidator()
  {
    RuleFor(x => x.Action).IsInEnum();
  }
}
