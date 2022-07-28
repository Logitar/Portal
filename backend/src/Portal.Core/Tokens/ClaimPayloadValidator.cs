using FluentValidation;
using Portal.Core.Tokens.Payloads;

namespace Portal.Core.Tokens
{
  internal class ClaimPayloadValidator : AbstractValidator<ClaimPayload>
  {
    public ClaimPayloadValidator()
    {
      RuleFor(x => x.Type)
        .NotEmpty();

      RuleFor(x => x.Value)
        .NotEmpty();
    }
  }
}
