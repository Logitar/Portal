namespace Logitar.Portal.Contracts.Dictionaries;

public record UpdateDictionaryPayload
{
  public IEnumerable<DictionaryEntryModification> Entries { get; set; } = Enumerable.Empty<DictionaryEntryModification>();
}
