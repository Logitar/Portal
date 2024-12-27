using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens.Validators;

internal class ClaimValidator : AbstractValidator<ClaimModel>
{
  public ClaimValidator()
  {
    RuleFor(x => x.Name).Identifier();
    RuleFor(x => x.Value).NotEmpty();
  }
}
