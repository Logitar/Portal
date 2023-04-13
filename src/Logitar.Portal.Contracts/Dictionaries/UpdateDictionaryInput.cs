namespace Logitar.Portal.Contracts.Dictionaries;

public record UpdateDictionaryInput
{
  public IEnumerable<Entry>? Entries { get; set; }
}
