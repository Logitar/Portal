using FluentValidation;
using Portal.Core.Emails.Messages.Payloads;

namespace Portal.Core.Emails.Messages
{
  internal class SendMessagePayloadValidator : AbstractValidator<SendMessagePayload>
  {
    public SendMessagePayloadValidator()
    {
      RuleFor(x => x.Template)
        .NotEmpty();

      RuleFor(x => x.Subject)
        .NotEmpty()
        .MaximumLength(256);

      RuleFor(x => x.Recipients)
        .Must(x => x.Any(y => y.Type == RecipientType.To));
      RuleForEach(x => x.Recipients)
        .SetValidator(new RecipientPayloadValidator());

      RuleForEach(x => x.Variables)
        .SetValidator(new VariablePayloadValidator());
    }
  }
}
