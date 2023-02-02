using FluentValidation;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens
{
  internal class CreateTokenPayloadValidator : AbstractValidator<CreateTokenPayload>
  {
    public CreateTokenPayloadValidator(ClaimPayloadValidator claimPayloadValidator)
    {
      RuleFor(x => x.Lifetime).GreaterThan(0);

      // TODO(fpion): WithErrorCode?
      // TODO(fpion): WithMessage?
      RuleFor(x => x.Purpose).Must(BeAValidPurpose);

      RuleFor(x => x.Email).EmailAddress();

      RuleForEach(x => x.Claims).SetValidator(claimPayloadValidator);
    }

    private static bool BeAValidPurpose(string? value) => value == null
      || value.Split('_').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetter));
  }
}
