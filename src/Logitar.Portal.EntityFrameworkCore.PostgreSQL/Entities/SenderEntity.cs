using Logitar.Portal.Core.Senders.Events;
using System.Text.Json;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;

internal class SenderEntity : AggregateEntity
{
  public SenderEntity(SenderCreated e, RealmEntity realm, ActorEntity actor) : base(e, actor)
  {
    Realm = realm;
    RealmId = realm.RealmId;

    Provider = e.Provider.ToString();

    Apply(e);
  }

  private SenderEntity() : base()
  {
  }

  public int SenderId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int RealmId { get; private set; }

  public bool IsDefault { get; private set; }

  public string EmailAddress { get; private set; } = string.Empty;
  public string? DisplayName { get; private set; }

  public string Provider { get; private set; } = string.Empty;
  public string? Settings { get; private set; }

  public void SetDefault(SenderSetDefault e, ActorEntity actor)
  {
    Update(e, actor);

    IsDefault = e.IsDefault;
  }

  public void Update(SenderUpdated e, ActorEntity actor)
  {
    base.Update(e, actor);

    Apply(e);
  }

  private void Apply(SenderSaved e)
  {
    EmailAddress = e.EmailAddress;
    DisplayName = e.DisplayName;

    Settings = e.Settings.Any() ? JsonSerializer.Serialize(e.Settings) : null;
  }
}
