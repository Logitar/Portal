using FluentValidation;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Core.Tokens.Validators;

internal class ClaimValidator : AbstractValidator<TokenClaim>
{
  public ClaimValidator()
  {
    RuleFor(x => x.Type).NotEmpty();

    RuleFor(x => x.Value).NotEmpty();

    RuleFor(x => x.ValueType).NullOrNotEmpty();
  }
}
