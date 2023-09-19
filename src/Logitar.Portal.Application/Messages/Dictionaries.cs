using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Messages;

public record Dictionaries
{
  private readonly DictionaryAggregate? _default = null;
  private readonly DictionaryAggregate? _fallback = null;
  private readonly DictionaryAggregate? _target = null;

  public Dictionaries(DictionaryAggregate? target = null, DictionaryAggregate? fallback = null, DictionaryAggregate? @default = null)
  {
    _default = @default;
    _fallback = fallback;
    _target = target;
  }

  public string Translate(string key)
  {
    return (_target?.Entries.TryGetValue(key, out string? value) == true
      || _fallback?.Entries.TryGetValue(key, out value) == true
      || _default?.Entries.TryGetValue(key, out value) == true)
      ? value : key;
  }
}
