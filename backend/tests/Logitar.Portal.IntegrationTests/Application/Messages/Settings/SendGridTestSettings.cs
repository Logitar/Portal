using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Application.Messages.Settings;

internal record SendGridTestSettings : ISendGridSettings
{
  public string ApiKey { get; set; }

  public string Address { get; set; }

  public SendGridTestSettings() : this(string.Empty, string.Empty)
  {
  }

  public SendGridTestSettings(string apiKey, string address)
  {
    ApiKey = apiKey;
    Address = address;
  }
}
