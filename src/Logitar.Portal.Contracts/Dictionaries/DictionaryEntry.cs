namespace Logitar.Portal.Contracts.Dictionaries;

public record DictionaryEntry
{
  public DictionaryEntry() : this(string.Empty, string.Empty)
  {
  }
  public DictionaryEntry(string key, string value)
  {
    Key = key;
    Value = value;
  }

  public string Key { get; set; }
  public string Value { get; set; }
}
