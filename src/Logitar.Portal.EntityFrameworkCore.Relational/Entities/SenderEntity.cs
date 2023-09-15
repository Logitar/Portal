using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record SenderEntity : AggregateEntity
{
  public SenderEntity(SenderCreatedEvent created) : base(created)
  {
    TenantId = created.TenantId;

    IsDefault = created.IsDefault;

    EmailAddress = created.EmailAddress;

    Provider = created.Provider;
  }

  private SenderEntity() : base()
  {
  }

  public int SenderId { get; private set; }

  public string? TenantId { get; private set; }

  public bool IsDefault { get; private set; }

  public string EmailAddress { get; private set; } = string.Empty;
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public ProviderType Provider { get; private set; }

  public Dictionary<string, string> Settings { get; private set; } = new();
  public string? SettingsSerialized
  {
    get => Settings.Any() ? JsonSerializer.Serialize(Settings) : null;
    private set
    {
      if (value == null)
      {
        Settings.Clear();
      }
      else
      {
        Settings = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new();
      }
    }
  }

  public RealmEntity? PasswordRecoveryInRealm { get; private set; }

  public List<MessageEntity> Messages { get; private set; } = new();

  public void Update(SenderUpdatedEvent updated)
  {
    base.Update(updated);

    if (updated.IsDefault.HasValue)
    {
      IsDefault = updated.IsDefault.Value;
    }

    if (updated.EmailAddress != null)
    {
      EmailAddress = updated.EmailAddress;
    }
    if (updated.DisplayName != null)
    {
      DisplayName = updated.DisplayName.Value;
    }
    if (updated.Description != null)
    {
      Description = updated.Description.Value;
    }

    foreach (KeyValuePair<string, string?> setting in updated.Settings)
    {
      if (setting.Value == null)
      {
        Settings.Remove(setting.Key);
      }
      else
      {
        Settings[setting.Key] = setting.Value;
      }
    }
  }
}
