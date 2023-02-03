namespace Logitar.Portal.Contracts.Dictionaries
{
  public record CreateDictionaryPayload
  {
    public string? Realm { get; set; }
    public string Locale { get; set; } = string.Empty;

    public IEnumerable<EntryPayload>? Entries { get; set; }
  }
}
