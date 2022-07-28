using FluentValidation;
using Portal.Core.Tokens.Payloads;

namespace Portal.Core.Tokens
{
  internal class CreateTokenPayloadValidator : AbstractValidator<CreateTokenPayload>
  {
    public CreateTokenPayloadValidator()
    {
      RuleFor(x => x.Lifetime)
        .GreaterThan(0);

      RuleFor(x => x.Purpose)
        .Must(BePurpose);

      RuleFor(x => x.Email)
        .EmailAddress();

      RuleForEach(x => x.Claims)
        .SetValidator(new ClaimPayloadValidator());
    }

    private static bool BePurpose(string? value) => value == null
      || value.Split('_').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetter));
  }
}
