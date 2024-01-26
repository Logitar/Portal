using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.Application.Validators;

internal class CustomAttributeContractValidator : AbstractValidator<CustomAttribute>
{
  public CustomAttributeContractValidator()
  {
    RuleFor(x => x.Key).SetValidator(new CustomAttributeKeyValidator());
    RuleFor(x => x.Value).SetValidator(new CustomAttributeValueValidator());
  }
}
