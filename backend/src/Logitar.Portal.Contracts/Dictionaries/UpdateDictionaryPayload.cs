namespace Logitar.Portal.Contracts.Dictionaries;

public record UpdateDictionaryPayload
{
  public string? Locale { get; set; }
  public List<DictionaryEntryModification> Entries { get; set; } = [];
}
