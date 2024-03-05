using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Domain.Dictionaries.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class DictionaryEntity : AggregateEntity
{
  public int DictionaryId { get; private set; }

  public string? TenantId { get; private set; }

  public string Locale { get; private set; } = string.Empty;
  public string LocaleNormalized
  {
    get => Locale.ToUpper();
    private set { }
  }

  public int EntryCount
  {
    get => Entries.Count;
    private set { }
  }
  public Dictionary<string, string> Entries { get; private set; } = [];
  public string? EntriesSerialized
  {
    get => Entries.Count == 0 ? null : JsonSerializer.Serialize(Entries);
    private set
    {
      if (value == null)
      {
        Entries.Clear();
      }
      else
      {
        Entries = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? [];
      }
    }
  }

  public DictionaryEntity(DictionaryCreatedEvent @event) : base(@event)
  {
    TenantId = @event.TenantId?.Value;

    Locale = @event.Locale.Code;
  }

  private DictionaryEntity() : base()
  {
  }

  public void SetLocale(DictionaryLocaleChangedEvent @event)
  {
    Update(@event);

    Locale = @event.Locale.Code;
  }

  public void Update(DictionaryUpdatedEvent @event)
  {
    base.Update(@event);

    foreach (KeyValuePair<string, string?> entry in @event.Entries)
    {
      if (entry.Value == null)
      {
        Entries.Remove(entry.Key);
      }
      else
      {
        Entries[entry.Key] = entry.Value;
      }
    }
  }
}
