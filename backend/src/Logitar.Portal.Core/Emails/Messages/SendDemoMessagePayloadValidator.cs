using FluentValidation;
using Logitar.Portal.Core.Emails.Messages.Payloads;

namespace Logitar.Portal.Core.Emails.Messages
{
  internal class SendDemoMessagePayloadValidator : AbstractValidator<SendDemoMessagePayload>
  {
    public SendDemoMessagePayloadValidator()
    {
      RuleForEach(x => x.Variables)
        .SetValidator(new VariablePayloadValidator());
    }
  }
}
