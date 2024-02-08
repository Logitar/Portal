using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Shared.Validators;
using Logitar.Portal.Application.Roles.Validators;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys.Validators;

internal class UpdateApiKeyValidator : AbstractValidator<UpdateApiKeyPayload>
{
  public UpdateApiKeyValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).SetValidator(new ExpirationValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeModificationValidator());
    RuleForEach(x => x.Roles).SetValidator(new RoleModificationValidator());
  }
}
