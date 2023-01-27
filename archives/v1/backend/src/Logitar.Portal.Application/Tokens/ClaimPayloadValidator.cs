using FluentValidation;
using Logitar.Portal.Core.Tokens.Payloads;

namespace Logitar.Portal.Application.Tokens
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
