using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders.Events;

namespace Logitar.Portal.Domain.Senders
{
  public class Sender : AggregateRoot
  {
    public Sender(AggregateId actorId, string emailAddress, ProviderType provider, Realm? realm = null,
      bool isDefault = false, string? displayName = null, Dictionary<string, string?>? settings = null) : base()
    {
      ApplyChange(new SenderCreatedEvent
      {
        RealmId = realm?.Id,
        IsDefault = isDefault,
        EmailAddress = emailAddress.Trim(),
        DisplayName = displayName?.CleanTrim(),
        Provider = provider,
        Settings = settings
      }, actorId);
    }
    private Sender() : base()
    {
    }

    public AggregateId? RealmId { get; private set; }

    public bool IsDefault { get; private set; }

    public string EmailAddress { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }

    public ProviderType Provider { get; private set; }
    public Dictionary<string, string?> Settings { get; private set; } = new();

    public void Delete(AggregateId actorId) => ApplyChange(new SenderDeletedEvent(), actorId);
    public void SetDefault(AggregateId actorId, bool isDefault = true)
    {
      ApplyChange(new SenderSetDefaultEvent
      {
        IsDefault = isDefault
      }, actorId);
    }
    public void Update(AggregateId actorId, string emailAddress, string? displayName = null, Dictionary<string, string?>? settings = null)
    {
      ApplyChange(new SenderUpdatedEvent
      {
        EmailAddress = emailAddress.Trim(),
        DisplayName = displayName?.CleanTrim(),
        Settings = settings
      }, actorId);
    }

    protected virtual void Apply(SenderCreatedEvent @event)
    {
      RealmId = @event.RealmId;

      IsDefault = @event.IsDefault;

      EmailAddress = @event.EmailAddress;
      DisplayName = @event.DisplayName;

      Provider = @event.Provider;
      Settings = @event.Settings ?? new();
    }
    protected virtual void Apply(SenderDeletedEvent @event)
    {
      Delete();
    }
    protected virtual void Apply(SenderSetDefaultEvent @event)
    {
      IsDefault = @event.IsDefault;
    }
    protected virtual void Apply(SenderUpdatedEvent @event)
    {
      EmailAddress = @event.EmailAddress;
      DisplayName = @event.DisplayName;

      Settings = @event.Settings ?? new();
    }

    public override string ToString() => DisplayName == null
      ? $"{EmailAddress} | {base.ToString()}"
      : $"{DisplayName} <{EmailAddress}> | {base.ToString()}";
  }
}
