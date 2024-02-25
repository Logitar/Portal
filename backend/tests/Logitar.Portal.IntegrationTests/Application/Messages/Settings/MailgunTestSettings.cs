namespace Logitar.Portal.Application.Messages.Settings;

internal record MailgunTestSettings
{
  public string ApiKey { get; set; }
  public string DomainName { get; set; }

  public string Address { get; set; }

  public MailgunTestSettings() : this(string.Empty, string.Empty, string.Empty)
  {
  }

  public MailgunTestSettings(string apiKey, string domainName, string address)
  {
    ApiKey = apiKey;
    DomainName = domainName;
    Address = address;
  }
}
