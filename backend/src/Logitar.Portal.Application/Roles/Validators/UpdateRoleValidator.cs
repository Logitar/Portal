using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Application.Roles.Validators;

internal class UpdateRoleValidator : AbstractValidator<UpdateRolePayload>
{
  public UpdateRoleValidator(IRoleSettings roleSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).SetValidator(new UniqueNameValidator(roleSettings.UniqueName)));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeModificationValidator());
  }
}
