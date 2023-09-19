namespace Logitar.Portal.Contracts.Dictionaries;

public record DictionaryEntryModification
{
  public DictionaryEntryModification() : this(string.Empty, null)
  {
  }
  public DictionaryEntryModification(string key, string? value)
  {
    Key = key;
    Value = value;
  }

  public string Key { get; set; }
  public string? Value { get; set; }
}
