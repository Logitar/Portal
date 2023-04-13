using FluentValidation;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Core.Tokens.Validators;

internal class ValidateTokenValidator : AbstractValidator<ValidateTokenInput>
{
  public ValidateTokenValidator()
  {
    RuleFor(x => x.Token).NotEmpty();

    RuleFor(x => x.Purpose).Purpose();

    RuleFor(x => x.Realm).NullOrNotEmpty();
    When(x => x.Realm == null, () => RuleFor(x => x.Secret).NotEmpty()
      .MinimumLength(256 / 8)
      .MaximumLength(512 / 8));

    RuleFor(x => x.Audience).NullOrNotEmpty();

    RuleFor(x => x.Issuer).NullOrNotEmpty();
  }
}
