using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.Application.Validators;

internal class CustomIdentifierContractValidator : AbstractValidator<CustomIdentifierModel>
{
  public CustomIdentifierContractValidator()
  {
    RuleFor(x => x.Key).Identifier();
    RuleFor(x => x.Value).CustomIdentifier();
  }
}
