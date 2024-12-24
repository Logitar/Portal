using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Dictionaries.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class DictionaryEntity : AggregateEntity
{
  public int DictionaryId { get; private set; }

  public Guid? TenantId { get; private set; }
  public Guid EntityId { get; private set; }

  public string Locale { get; private set; } = string.Empty;
  public string LocaleNormalized
  {
    get => Helper.Normalize(Locale);
    private set { }
  }

  public int EntryCount { get; private set; }
  public string? Entries { get; private set; }

  public DictionaryEntity(DictionaryCreated @event) : base(@event)
  {
    DictionaryId dictionaryId = new(@event.StreamId);
    TenantId = dictionaryId.TenantId?.ToGuid();
    EntityId = dictionaryId.EntityId.ToGuid();

    Locale = @event.Locale.Code;
  }

  private DictionaryEntity() : base()
  {
  }

  public void SetLocale(DictionaryLocaleChanged @event)
  {
    Update(@event);

    Locale = @event.Locale.Code;
  }

  public void Update(DictionaryUpdated @event)
  {
    base.Update(@event);

    Dictionary<string, string> entries = GetEntries();
    foreach (KeyValuePair<Identifier, string?> entry in @event.Entries)
    {
      if (entry.Value == null)
      {
        entries.Remove(entry.Key.Value);
      }
      else
      {
        entries[entry.Key.Value] = entry.Value;
      }
    }
    SetEntries(entries);
  }

  public Dictionary<string, string> GetEntries()
  {
    return (Entries == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(Entries)) ?? [];
  }
  private void SetEntries(Dictionary<string, string> entries)
  {
    EntryCount = entries.Count;
    Entries = entries.Count < 1 ? null : JsonSerializer.Serialize(entries);
  }

  public override string ToString() => $"{Locale} | {base.ToString()}";
}
