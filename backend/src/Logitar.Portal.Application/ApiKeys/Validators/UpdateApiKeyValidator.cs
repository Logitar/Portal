using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Roles.Validators;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys.Validators;

internal class UpdateApiKeyValidator : AbstractValidator<UpdateApiKeyPayload>
{
  public UpdateApiKeyValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).Future());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeModificationValidator());
    RuleForEach(x => x.Roles).SetValidator(new RoleModificationValidator());
  }
}
