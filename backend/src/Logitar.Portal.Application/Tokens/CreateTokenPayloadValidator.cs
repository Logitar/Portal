using FluentValidation;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens
{
  internal class CreateTokenPayloadValidator : AbstractValidator<CreateTokenPayload>
  {
    public CreateTokenPayloadValidator(ClaimPayloadValidator claimPayloadValidator)
    {
      RuleFor(x => x.Lifetime).GreaterThan(0);

      RuleFor(x => x.Purpose).Purpose();

      RuleFor(x => x.Email).EmailAddress();

      RuleForEach(x => x.Claims).SetValidator(claimPayloadValidator);
    }
  }
}
