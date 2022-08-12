namespace Logitar.Portal.Core.Dictionaries.Payloads
{
  public class CreateDictionaryPayload : SaveDictionaryPayload
  {
    public string? Realm { get; set; }
    public string Locale { get; set; } = null!;
  }
}
