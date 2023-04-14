using FluentValidation;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Core.Tokens.Validators;

internal class CreateTokenValidator : AbstractValidator<CreateTokenInput>
{
  public CreateTokenValidator()
  {
    RuleFor(x => x.Lifetime).GreaterThan(0);

    RuleFor(x => x.Purpose).Purpose();

    RuleFor(x => x.Realm).NullOrNotEmpty();

    RuleFor(x => x.Secret).NullOrNotEmpty()
      .MinimumLength(256 / 8)
      .MaximumLength(512 / 8);

    RuleFor(x => x.Audience).NullOrNotEmpty();

    RuleFor(x => x.Issuer).NullOrNotEmpty();

    RuleForEach(x => x.Claims).SetValidator(new ClaimValidator());
  }
}
