using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.Application.Validators;

internal class CustomAttributeModificationValidator : AbstractValidator<CustomAttributeModification>
{
  public CustomAttributeModificationValidator()
  {
    RuleFor(x => x.Key).SetValidator(new CustomAttributeKeyValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Value), () => RuleFor(x => x.Value!).SetValidator(new CustomAttributeValueValidator()));
  }
}
