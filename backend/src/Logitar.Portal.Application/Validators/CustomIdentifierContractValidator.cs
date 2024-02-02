using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.Application.Validators;

internal class CustomIdentifierContractValidator : AbstractValidator<CustomIdentifier>
{
  public CustomIdentifierContractValidator()
  {
    RuleFor(x => x.Key).SetValidator(new CustomIdentifierKeyValidator());
    RuleFor(x => x.Value).SetValidator(new CustomIdentifierValueValidator());
  }
}
