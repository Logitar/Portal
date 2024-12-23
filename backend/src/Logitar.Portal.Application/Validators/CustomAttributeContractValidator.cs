using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.Application.Validators;

internal class CustomAttributeContractValidator : AbstractValidator<CustomAttribute>
{
  public CustomAttributeContractValidator()
  {
    RuleFor(x => x.Key).Identifier());
    RuleFor(x => x.Value).NotEmpty());
  }
}
