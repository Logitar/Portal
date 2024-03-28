using FluentValidation;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders.Twilio;

public class TwilioSettingsValidator : AbstractValidator<ITwilioSettings>
{
  public TwilioSettingsValidator()
  {
    RuleFor(x => x.AccountSid).NotEmpty();
    RuleFor(x => x.AuthenticationToken).NotEmpty();
  }
}
