using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Application.Roles.Validators;

internal class ReplaceRoleValidator : AbstractValidator<ReplaceRolePayload>
{
  public ReplaceRoleValidator(IRoleSettings roleSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(roleSettings.UniqueName));
    When(x => x.DisplayName != null, () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => x.Description != null, () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
