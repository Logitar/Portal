namespace Logitar.Portal.Contracts.Senders;

public record SendGridSettings : ISendGridSettings
{
  public string ApiKey { get; set; }

  public SendGridSettings() : this(string.Empty)
  {
  }

  public SendGridSettings(string apiKey)
  {
    ApiKey = apiKey;
  }
}
