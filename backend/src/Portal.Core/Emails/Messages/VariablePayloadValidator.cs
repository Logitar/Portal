using FluentValidation;
using Portal.Core.Emails.Messages.Payloads;

namespace Portal.Core.Emails.Messages
{
  internal class VariablePayloadValidator : AbstractValidator<VariablePayload>
  {
    public VariablePayloadValidator()
    {
      RuleFor(x => x.Key)
        .NotEmpty();
    }
  }
}
