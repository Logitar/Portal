using Logitar.Portal.Domain.Dictionaries.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record DictionaryEntity : AggregateEntity
{
  public DictionaryEntity(DictionaryCreatedEvent created) : base(created)
  {
    TenantId = created.TenantId;

    Locale = created.Locale.Code;
  }

  private DictionaryEntity() : base()
  {
  }

  public int DictionaryId { get; private set; }

  public string? TenantId { get; private set; }

  public string Locale { get; private set; } = string.Empty;

  public Dictionary<string, string> Entries { get; private set; } = new();
  public string? EntriesSerialized
  {
    get => Entries.Any() ? JsonSerializer.Serialize(Entries) : null;
    private set
    {
      if (value == null)
      {
        Entries.Clear();
      }
      else
      {
        Entries = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new();
      }
    }
  }
  public int EntryCount
  {
    get => Entries.Count;
    private set { }
  }

  public void Update(DictionaryUpdatedEvent updated)
  {
    base.Update(updated);

    foreach (KeyValuePair<string, string?> entry in updated.Entries)
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
