using Logitar.Portal.Domain.Dictionaries.Events;
using System.Globalization;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class DictionaryEntity : AggregateEntity
  {
    public DictionaryEntity(DictionaryCreatedEvent @event, Actor actor, RealmEntity? realm = null) : base(@event, actor)
    {
      Realm = realm;
      RealmId = realm?.RealmId;

      Locale = @event.Locale;

      Entries = @event.Entries?.Any() == true ? JsonSerializer.Serialize(@event.Entries) : null;
    }
    private DictionaryEntity() : base()
    {
    }

    public int DictionaryId { get; private set; }

    public RealmEntity? Realm { get; private set; }
    public int? RealmId { get; private set; }

    public CultureInfo Locale { get; private set; } = CultureInfo.InvariantCulture;

    public string? Entries { get; private set; }

    public void Update(DictionaryUpdatedEvent @event, Actor actor)
    {
      base.Update(@event, actor);

      Entries = @event.Entries?.Any() == true ? JsonSerializer.Serialize(@event.Entries) : null;
    }
  }
}
