using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Emails.Senders.Events;
using Logitar.Portal.Core.Emails.Senders.Payloads;
using System.Text.Json;

namespace Logitar.Portal.Core.Emails.Senders
{
  public class Sender : Aggregate
  {
    public Sender(CreateSenderPayload payload, Guid userId, bool isDefault = false, Realm? realm = null)
    {
      ApplyChange(new CreatedEvent(isDefault, payload, userId));

      Realm = realm;
      RealmSid = realm?.Sid;
    }
    private Sender()
    {
    }

    public Realm? Realm { get; private set; }
    public int? RealmSid { get; private set; }

    public bool IsDefault { get; private set; }

    public string EmailAddress { get; private set; } = null!;
    public string? DisplayName { get; private set; }

    public ProviderType Provider { get; private set; }
    public Dictionary<string, string?> Settings { get; private set; } = new();
    public string? SettingsSerialized
    {
      get => Settings.Any() ? JsonSerializer.Serialize(Settings) : null;
      private set
      {
        Settings.Clear();

        if (value != null)
        {
          var settings = JsonSerializer.Deserialize<Dictionary<string, string?>>(value);
          if (settings != null)
          {
            foreach (var (key, setting) in settings)
            {
              Settings.Add(key, setting);
            }
          }
        }
      }
    }

    public void Delete(Guid userId) => ApplyChange(new DeletedEvent(userId));
    public void SetDefault(Guid userId, bool isDefault = true) => ApplyChange(new SetDefaultEvent(userId, isDefault));
    public void Update(UpdateSenderPayload payload, Guid userId) => ApplyChange(new UpdatedEvent(payload, userId));

    protected virtual void Apply(CreatedEvent @event)
    {
      IsDefault = @event.IsDefault;
      Provider = @event.Payload.Provider;

      Apply(@event.Payload);
    }
    protected virtual void Apply(DeletedEvent @event)
    {
    }
    protected virtual void Apply(SetDefaultEvent @event)
    {
      IsDefault = @event.IsDefault;
    }
    protected virtual void Apply(UpdatedEvent @event)
    {
      Apply(@event.Payload);
    }

    private void Apply(SaveSenderPayload payload)
    {
      EmailAddress = payload.EmailAddress;
      DisplayName = payload.DisplayName?.CleanTrim();

      Settings.Clear();
      if (payload.Settings != null)
      {
        foreach (SettingPayload setting in payload.Settings)
        {
          Settings.Add(setting.Key, setting.Value);
        }
      }
    }

    public override string ToString() => DisplayName == null
      ? $"{EmailAddress} | {base.ToString()}"
      : $"{DisplayName} <{EmailAddress}> | {base.ToString()}";
  }
}
