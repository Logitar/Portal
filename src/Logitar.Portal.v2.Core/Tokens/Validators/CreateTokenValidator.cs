using FluentValidation;
using Logitar.Portal.v2.Contracts.Tokens;

namespace Logitar.Portal.v2.Core.Tokens.Validators;

internal class CreateTokenValidator : AbstractValidator<CreateTokenInput>
{
  public CreateTokenValidator()
  {
    RuleFor(x => x.Lifetime).GreaterThan(0);

    RuleFor(x => x.Purpose).Purpose();

    RuleFor(x => x.Realm).NullOrNotEmpty();
    When(x => x.Realm == null, () => RuleFor(x => x.Secret).NotEmpty()
      .MinimumLength(256 / 8)
      .MaximumLength(512 / 8));

    RuleFor(x => x.Audience).NullOrNotEmpty();

    RuleFor(x => x.Issuer).NullOrNotEmpty();

    RuleForEach(x => x.Claims).SetValidator(new ClaimValidator());
  }
}
