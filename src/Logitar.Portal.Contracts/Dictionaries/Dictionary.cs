using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Dictionaries;

public record Dictionary : Aggregate
{
  public Guid Id { get; set; }

  public Realm? Realm { get; set; }
  public Locale Locale { get; set; } = new();

  public IEnumerable<DictionaryEntry> Entries { get; set; } = Enumerable.Empty<DictionaryEntry>();
  public int EntryCount { get; set; }
}
