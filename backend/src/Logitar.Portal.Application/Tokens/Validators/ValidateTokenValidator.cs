using FluentValidation;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Application.Tokens.Validators;

internal class ValidateTokenValidator : AbstractValidator<ValidateTokenPayload>
{
  public ValidateTokenValidator()
  {
    RuleFor(x => x.Token).NotEmpty();

    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).SetValidator(new JwtSecretValidator()));
  }
}
