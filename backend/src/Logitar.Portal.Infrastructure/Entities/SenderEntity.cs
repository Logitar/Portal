using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.Events;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class SenderEntity : AggregateEntity
  {
    public SenderEntity(SenderCreatedEvent @event, RealmEntity? realm) : base(@event)
    {
      Realm = realm;
      RealmId = realm?.RealmId;

      IsDefault = @event.IsDefault;

      EmailAddress = @event.EmailAddress;
      DisplayName = @event.DisplayName;

      Provider = @event.Provider;
      Settings = @event.Settings?.Any() == true ? JsonSerializer.Serialize(@event.Settings) : null;
    }
    private SenderEntity() : base()
    {
    }

    public int SenderId { get; private set; }

    public RealmEntity? Realm { get; private set; }
    public int? RealmId { get; private set; }

    public bool IsDefault { get; private set; }

    public string EmailAddress { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }

    public ProviderType Provider { get; private set; }
    public string? Settings { get; private set; }

    public void SetDefault(SenderSetDefaultEvent @event)
    {
      base.Update(@event);

      IsDefault = @event.IsDefault;
    }
    public void Update(SenderUpdatedEvent @event)
    {
      base.Update(@event);

      EmailAddress = @event.EmailAddress;
      DisplayName = @event.DisplayName;

      Settings = @event.Settings?.Any() == true ? JsonSerializer.Serialize(@event.Settings) : null;
    }
  }
}
