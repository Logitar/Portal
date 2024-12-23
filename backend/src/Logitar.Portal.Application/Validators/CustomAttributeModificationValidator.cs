using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.Application.Validators;

internal class CustomAttributeModificationValidator : AbstractValidator<CustomAttributeModification>
{
  public CustomAttributeModificationValidator()
  {
    RuleFor(x => x.Key).Identifier();
  }
}
