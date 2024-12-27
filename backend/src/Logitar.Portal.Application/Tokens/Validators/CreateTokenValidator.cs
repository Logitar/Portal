using FluentValidation;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Tokens.Validators;

internal class CreateTokenValidator : AbstractValidator<CreateTokenPayload>
{
  public CreateTokenValidator()
  {
    When(x => x.LifetimeSeconds.HasValue, () => RuleFor(x => x.LifetimeSeconds!.Value).GreaterThan(0));
    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).JwtSecret());

    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    RuleForEach(x => x.Claims).SetValidator(new ClaimValidator());
  }
}
