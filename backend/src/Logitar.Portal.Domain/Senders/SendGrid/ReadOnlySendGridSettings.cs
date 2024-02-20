using FluentValidation;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders.SendGrid;

public record ReadOnlySendGridSettings : SenderSettings, ISendGridSettings
{
  [JsonIgnore]
  public override SenderProvider Provider { get; } = SenderProvider.SendGrid;

  public string ApiKey { get; }

  public ReadOnlySendGridSettings(ISendGridSettings sendGrid) : this(sendGrid.ApiKey)
  {
  }

  [JsonConstructor]
  public ReadOnlySendGridSettings(string apiKey)
  {
    ApiKey = apiKey.Trim();
    new SendGridSettingsValidator().ValidateAndThrow(this);
  }
}
