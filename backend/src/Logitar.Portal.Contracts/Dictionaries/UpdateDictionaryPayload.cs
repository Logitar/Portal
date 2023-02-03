namespace Logitar.Portal.Contracts.Dictionaries
{
  public record UpdateDictionaryPayload
  {
    public IEnumerable<EntryPayload>? Entries { get; set; }
  }
}
