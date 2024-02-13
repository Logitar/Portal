namespace Logitar.Portal.Contracts.Dictionaries;

public record DictionaryEntry
{
  public string Key { get; set; }
  public string Value { get; set; }

  public DictionaryEntry() : this(string.Empty, string.Empty)
  {
  }

  public DictionaryEntry(KeyValuePair<string, string> pair) : this(pair.Key, pair.Value)
  {
  }

  public DictionaryEntry(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
