namespace Logitar.Portal.Contracts.Senders;

public record MailgunSettings : IMailgunSettings
{
  public string ApiKey { get; set; }
  public string DomainName { get; set; }

  public MailgunSettings() : this(string.Empty, string.Empty)
  {
  }

  public MailgunSettings(string apiKey, string domainName)
  {
    ApiKey = apiKey;
    DomainName = domainName;
  }
}
