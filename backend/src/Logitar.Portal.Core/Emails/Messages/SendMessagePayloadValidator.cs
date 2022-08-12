using FluentValidation;
using Logitar.Portal.Core.Emails.Messages.Payloads;

namespace Logitar.Portal.Core.Emails.Messages
{
  internal class SendMessagePayloadValidator : AbstractValidator<SendMessagePayload>
  {
    public SendMessagePayloadValidator()
    {
      RuleFor(x => x.Template)
        .NotEmpty();

      RuleFor(x => x.Locale)
        .Must(ValidationRules.BeAValidCulture);

      RuleFor(x => x.Recipients)
        .Must(x => x.Any(y => y.Type == RecipientType.To));
      RuleForEach(x => x.Recipients)
        .SetValidator(new RecipientPayloadValidator());

      RuleForEach(x => x.Variables)
        .SetValidator(new VariablePayloadValidator());
    }
  }
}
