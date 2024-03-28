namespace Logitar.Portal.Contracts.Senders;

public record TwilioSettings : ITwilioSettings
{
  public string AccountSid { get; set; }
  public string AuthenticationToken { get; set; }

  public TwilioSettings() : this(string.Empty, string.Empty)
  {
  }

  public TwilioSettings(string accountSid, string authenticationToken)
  {
    AccountSid = accountSid;
    AuthenticationToken = authenticationToken;
  }
}
