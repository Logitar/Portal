using Logitar.Portal.v2.Core.Dictionaries;

namespace Logitar.Portal.v2.Core.Messages;

public record Dictionaries(DictionaryAggregate? Default, DictionaryAggregate? Fallback, DictionaryAggregate? Preferred)
{
  public string GetEntry(string key)
  {
    if (Preferred?.Entries.TryGetValue(key, out string? value) == true
      || Fallback?.Entries.TryGetValue(key, out value) == true
      || Default?.Entries.TryGetValue(key, out value) == true)
    {
      return value;
    }

    return key;
  }
}
