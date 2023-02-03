using Logitar.Portal.Domain.Dictionaries.Events;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class DictionaryEntity : AggregateEntity
  {
    public DictionaryEntity(DictionaryCreatedEvent @event, RealmEntity? realm) : base(@event)
    {
      Realm = realm;
      RealmId = realm?.RealmId;

      Locale = @event.LocaleName;

      Entries = @event.Entries?.Any() == true ? JsonSerializer.Serialize(@event.Entries) : null;
    }
    private DictionaryEntity() : base()
    {
    }

    public int DictionaryId { get; private set; }

    public RealmEntity? Realm { get; private set; }
    public int? RealmId { get; private set; }

    public string Locale { get; private set; } = string.Empty;

    public string? Entries { get; private set; }

    public void Update(DictionaryUpdatedEvent @event)
    {
      Entries = @event.Entries?.Any() == true ? JsonSerializer.Serialize(@event.Entries) : null;
    }
  }
}
