using FluentValidation;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders.Twilio;

public record ReadOnlyTwilioSettings : SenderSettings, ITwilioSettings
{
  [JsonIgnore]
  public override SenderProvider Provider { get; } = SenderProvider.Twilio;

  public string AccountSid { get; }
  public string AuthenticationToken { get; }

  public ReadOnlyTwilioSettings(ITwilioSettings twilio) : this(twilio.AccountSid, twilio.AuthenticationToken)
  {
  }

  [JsonConstructor]
  public ReadOnlyTwilioSettings(string accountSid, string authenticationToken)
  {
    AccountSid = accountSid.Trim();
    AuthenticationToken = authenticationToken.Trim();
    new TwilioSettingsValidator().ValidateAndThrow(this);
  }
}
