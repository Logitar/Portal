using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class SenderEntity : AggregateEntity
{
  public int SenderId { get; private set; }

  public Guid? TenantId { get; private set; }
  public Guid EntityId { get; private set; }

  public bool IsDefault { get; private set; }

  public string? EmailAddress { get; private set; }
  public string? PhoneNumber { get; private set; }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public SenderProvider Provider { get; private set; }
  public string? Settings { get; private set; }

  public List<MessageEntity> Messages { get; private set; } = [];

  public SenderEntity(EmailSenderCreated @event) : this((SenderCreated)@event)
  {
  }
  public SenderEntity(SenderCreated @event) : base(@event)
  {
    SenderId senderId = new(@event.StreamId);
    TenantId = senderId.TenantId?.ToGuid();
    EntityId = senderId.EntityId.ToGuid();

    EmailAddress = @event.Email.Address;

    Provider = @event.Provider;
  }

  public SenderEntity(SmsSenderCreated @event) : base(@event)
  {
    SenderId senderId = new(@event.StreamId);
    TenantId = senderId.TenantId?.ToGuid();
    EntityId = senderId.EntityId.ToGuid();

    PhoneNumber = @event.Phone.Number;

    Provider = @event.Provider;
  }

  private SenderEntity() : base()
  {
  }

  public void SetDefault(SenderSetDefault @event)
  {
    Update(@event);

    IsDefault = @event.IsDefault;
  }

  public void SetMailgunSettings(SenderMailgunSettingsChanged @event)
  {
    Dictionary<string, string> settings = GetSettings();
    settings.Clear();
    settings[nameof(IMailgunSettings.ApiKey)] = @event.Settings.ApiKey;
    settings[nameof(IMailgunSettings.DomainName)] = @event.Settings.DomainName;
    SetSettings(settings);
  }
  public void SetSendGridSettings(SenderSendGridSettingsChanged @event)
  {
    Dictionary<string, string> settings = GetSettings();
    settings.Clear();
    settings.Clear();
    settings[nameof(ISendGridSettings.ApiKey)] = @event.Settings.ApiKey;
    SetSettings(settings);
  }
  public void SetTwilioSettings(SenderTwilioSettingsChanged @event)
  {
    Dictionary<string, string> settings = GetSettings();
    settings.Clear();
    settings.Clear();
    settings[nameof(ITwilioSettings.AccountSid)] = @event.Settings.AccountSid;
    settings[nameof(ITwilioSettings.AuthenticationToken)] = @event.Settings.AuthenticationToken;
    SetSettings(settings);
  }

  public void Update(SenderUpdated @event)
  {
    base.Update(@event);

    if (@event.Email != null)
    {
      EmailAddress = @event.Email.Address;
    }
    if (@event.Phone != null)
    {
      PhoneNumber = @event.Phone.Number;
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

  public Dictionary<string, string> GetSettings()
  {
    return (Settings == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(Settings)) ?? [];
  }
  private void SetSettings(Dictionary<string, string> settings)
  {
    Settings = settings.Count < 1 ? null : JsonSerializer.Serialize(settings);
  }

  public override string ToString()
  {
    if (EmailAddress == null)
    {
      return $"{PhoneNumber} | {base.ToString()}";
    }

    return DisplayName == null ? $"{EmailAddress} | {base.ToString()}" : $"{DisplayName} <{EmailAddress}> | {base.ToString()}";
  }
}
