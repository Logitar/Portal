using FluentValidation;
using Logitar.Portal.Core.Emails.Messages.Payloads;

namespace Logitar.Portal.Application.Emails.Messages
{
  internal class SendDemoMessagePayloadValidator : AbstractValidator<SendDemoMessagePayload>
  {
    public SendDemoMessagePayloadValidator()
    {
      RuleFor(x => x.Locale)
        .Must(ValidationRules.BeAValidCulture);

      RuleForEach(x => x.Variables)
        .SetValidator(new VariablePayloadValidator());
    }
  }
}
