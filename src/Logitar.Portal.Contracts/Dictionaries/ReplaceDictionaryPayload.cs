namespace Logitar.Portal.Contracts.Dictionaries;

public record ReplaceDictionaryPayload
{
  public IEnumerable<DictionaryEntry> Entries { get; set; } = Enumerable.Empty<DictionaryEntry>();
}
