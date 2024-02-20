using FluentValidation;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders.SendGrid;

public record ReadOnlySendGridSettings : SenderSettings, ISendGridSettings
{
  [JsonIgnore]
  public override SenderProvider Provider { get; } = SenderProvider.SendGrid;

  public string ApiKey { get; }

  public ReadOnlySendGridSettings() : this(string.Empty)
  {
  }

  public ReadOnlySendGridSettings(ISendGridSettings sendGrid) : this(sendGrid.ApiKey)
  {
  }

  public ReadOnlySendGridSettings(string apiKey)
  {
    ApiKey = apiKey;
    new SendGridSettingsValidator().ValidateAndThrow(this);
  }
}
