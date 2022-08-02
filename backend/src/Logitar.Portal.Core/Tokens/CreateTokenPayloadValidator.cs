using FluentValidation;
using Logitar.Portal.Core.Tokens.Payloads;

namespace Logitar.Portal.Core.Tokens
{
  internal class CreateTokenPayloadValidator : AbstractValidator<CreateTokenPayload>
  {
    public CreateTokenPayloadValidator()
    {
      RuleFor(x => x.Lifetime)
        .GreaterThan(0);

      RuleFor(x => x.Purpose)
        .Must(BeAValidPurpose);

      RuleFor(x => x.Email)
        .EmailAddress();

      RuleForEach(x => x.Claims)
        .SetValidator(new ClaimPayloadValidator());
    }

    private static bool BeAValidPurpose(string? value) => value == null
      || value.Split('_').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetter));
  }
}
