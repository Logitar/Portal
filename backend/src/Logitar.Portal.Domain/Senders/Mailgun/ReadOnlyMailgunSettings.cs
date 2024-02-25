using FluentValidation;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders.Mailgun;

public record ReadOnlyMailgunSettings : SenderSettings, IMailgunSettings
{
  [JsonIgnore]
  public override SenderProvider Provider { get; } = SenderProvider.Mailgun;

  public string ApiKey { get; }
  public string DomainName { get; }

  public ReadOnlyMailgunSettings(IMailgunSettings mailgun) : this(mailgun.ApiKey, mailgun.DomainName)
  {
  }

  [JsonConstructor]
  public ReadOnlyMailgunSettings(string apiKey, string domainName)
  {
    ApiKey = apiKey.Trim();
    DomainName = domainName.Trim();
    new MailgunSettingsValidator().ValidateAndThrow(this);
  }
}
