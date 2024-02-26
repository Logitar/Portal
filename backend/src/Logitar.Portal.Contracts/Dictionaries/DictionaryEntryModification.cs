namespace Logitar.Portal.Contracts.Dictionaries;

public record DictionaryEntryModification
{
  public string Key { get; set; }
  public string? Value { get; set; }

  public DictionaryEntryModification() : this(string.Empty, null)
  {
  }

  public DictionaryEntryModification(KeyValuePair<string, string?> pair) : this(pair.Key, pair.Value)
  {
  }

  public DictionaryEntryModification(string key, string? value)
  {
    Key = key;
    Value = value;
  }
}
