using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Application.Messages.Settings;

internal record TwilioTestSettings : ITwilioSettings
{
  public string AccountSid { get; set; }
  public string AuthenticationToken { get; set; }

  public string PhoneNumber { get; set; }

  public TwilioTestSettings() : this(string.Empty, string.Empty, string.Empty)
  {
  }

  public TwilioTestSettings(string accountSid, string authenticationToken, string phoneNumber)
  {
    AccountSid = accountSid;
    AuthenticationToken = authenticationToken;
    PhoneNumber = phoneNumber;
  }
}
