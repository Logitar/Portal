namespace Logitar.Portal.Contracts.Dictionaries;

public record ReplaceDictionaryPayload
{
  public string Locale { get; set; }
  public List<DictionaryEntry> Entries { get; set; }

  public ReplaceDictionaryPayload() : this(string.Empty)
  {
  }

  public ReplaceDictionaryPayload(string locale)
  {
    Locale = locale;
    Entries = [];
  }
}
