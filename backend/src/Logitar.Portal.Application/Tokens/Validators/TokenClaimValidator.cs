using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens.Validators;

internal class TokenClaimValidator : AbstractValidator<ClaimModel>
{
  public TokenClaimValidator()
  {
    RuleFor(x => x.Name).Identifier();
    RuleFor(x => x.Value).NotEmpty();
  }
}
