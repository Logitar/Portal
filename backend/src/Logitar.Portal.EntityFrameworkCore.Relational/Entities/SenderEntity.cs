using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class SenderEntity : AggregateEntity
{
  public int SenderId { get; private set; }

  public string? TenantId { get; private set; }

  public bool IsDefault { get; private set; }

  public string EmailAddress { get; private set; } = string.Empty;
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public SenderProvider Provider { get; private set; }
  public Dictionary<string, string> Settings { get; private set; } = [];
  public string? SettingsSerialized
  {
    get => Settings.Count == 0 ? null : JsonSerializer.Serialize(Settings);
    private set
    {
      if (value == null)
      {
        Settings.Clear();
      }
      else
      {
        Settings = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? [];
      }
    }
  }

  public List<MessageEntity> Messages { get; private set; } = [];

  public SenderEntity(SenderCreatedEvent @event) : base(@event)
  {
    TenantId = @event.TenantId?.Value;

    EmailAddress = @event.Email.Address;

    Provider = @event.Provider;
  }

  private SenderEntity() : base()
  {
  }

  public void SetDefault(SenderSetDefaultEvent @event)
  {
    Update(@event);

    IsDefault = @event.IsDefault;
  }

  public void SetMailgunSettings(SenderMailgunSettingsChangedEvent @event)
  {
    Settings.Clear();
    Settings[nameof(IMailgunSettings.ApiKey)] = @event.Settings.ApiKey;
    Settings[nameof(IMailgunSettings.DomainName)] = @event.Settings.DomainName;
  }
  public void SetSendGridSettings(SenderSendGridSettingsChangedEvent @event)
  {
    Settings.Clear();
    Settings[nameof(ISendGridSettings.ApiKey)] = @event.Settings.ApiKey;
  }

  public void Update(SenderUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Email != null)
    {
      EmailAddress = @event.Email.Address;
    }
    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }
}
