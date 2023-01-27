namespace Logitar.Portal.Core.Dictionaries.Payloads
{
  public abstract class SaveDictionaryPayload
  {
    public IEnumerable<EntryPayload>? Entries { get; set; }
  }
}
