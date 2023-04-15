using Logitar.Portal.Core.Dictionaries.Events;
using System.Text.Json;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;

internal class DictionaryEntity : AggregateEntity
{
  public DictionaryEntity(DictionaryCreated e, RealmEntity? realm, ActorEntity actor) : base(e, actor)
  {
    Realm = realm;
    RealmId = realm?.RealmId;

    Locale = e.Locale.Name;

    Apply(e);
  }

  private DictionaryEntity() : base()
  {
  }

  public int DictionaryId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public string Locale { get; private set; } = string.Empty;

  public string? Entries { get; private set; }
  public int EntryCount { get; private set; }

  public void Update(DictionaryUpdated e, ActorEntity actor)
  {
    base.Update(e, actor);

    Apply(e);
  }

  private void Apply(DictionarySaved e)
  {
    Entries = e.Entries.Any() ? JsonSerializer.Serialize(e.Entries) : null;
    EntryCount = e.Entries.Count;
  }
}
