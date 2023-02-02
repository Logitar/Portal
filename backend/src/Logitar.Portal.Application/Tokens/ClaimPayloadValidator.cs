using FluentValidation;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens
{
  internal class ClaimPayloadValidator : AbstractValidator<ClaimPayload>
  {
    public ClaimPayloadValidator()
    {
      RuleFor(x => x.Type).NotEmpty();

      RuleFor(x => x.Value).NotEmpty();

      RuleFor(x => x.ValueType).NullOrNotEmpty();
    }
  }
}
