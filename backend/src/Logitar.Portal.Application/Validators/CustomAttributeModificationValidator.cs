using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.Application.Validators;

internal class CustomAttributeModificationValidator : AbstractValidator<CustomAttributeModification>
{
  public CustomAttributeModificationValidator()
  {
    RuleFor(x => x.Key).SetValidator(new CustomAttributeKeyValidator());
    When(x => x.Value != null, () => RuleFor(x => x.Value!).SetValidator(new CustomAttributeValueValidator()));
  }
}
