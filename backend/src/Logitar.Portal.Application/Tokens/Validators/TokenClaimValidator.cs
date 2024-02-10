using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens.Validators;

internal class TokenClaimValidator : AbstractValidator<TokenClaim>
{
  public TokenClaimValidator()
  {
    RuleFor(x => x.Name).SetValidator(new IdentifierValidator());
    RuleFor(x => x.Value).NotEmpty();
  }
}
