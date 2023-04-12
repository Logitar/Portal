namespace Logitar.Portal.v2.Contracts.Dictionaries;

public record UpdateDictionaryInput
{
  public IEnumerable<Entry>? Entries { get; set; }
}
