using FluentValidation;
using Logitar.Portal.Core.Emails.Messages.Payloads;

namespace Logitar.Portal.Core.Emails.Messages
{
  internal class VariablePayloadValidator : AbstractValidator<VariablePayload>
  {
    public VariablePayloadValidator()
    {
      RuleFor(x => x.Key)
        .NotEmpty()
        .MaximumLength(256)
        .Must(ValidationRules.BeAValidIdentifier);
    }
  }
}
