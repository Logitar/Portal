namespace Logitar.Portal.Contracts.Dictionaries;

public record SearchDictionariesPayload : SearchPayload
{
  public string? Realm { get; set; }

  public new IEnumerable<DictionarySortOption> Sort { get; set; } = Enumerable.Empty<DictionarySortOption>();
}
