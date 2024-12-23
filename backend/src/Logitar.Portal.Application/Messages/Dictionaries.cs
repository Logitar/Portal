using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Messages;

public record Dictionaries
{
  public DictionaryAggregate? Target { get; }
  public DictionaryAggregate? Fallback { get; }
  public DictionaryAggregate? Default { get; }

  public Dictionaries()
  {
  }

  public Dictionaries(IReadOnlyDictionary<Locale, DictionaryAggregate> dictionaries, Locale? targetLocale = null, Locale? defaultLocale = null) : this()
  {
    if (targetLocale != null)
    {
      if (dictionaries.TryGetValue(targetLocale, out DictionaryAggregate? target))
      {
        Target = target;
      }

      if (!string.IsNullOrEmpty(targetLocale.Culture.Parent?.Name))
      {
        Locale fallbackLocale = new(targetLocale.Culture.Parent.Name);
        if (dictionaries.TryGetValue(fallbackLocale, out DictionaryAggregate? fallback))
        {
          Fallback = fallback;
        }
      }
    }

    if (defaultLocale != null && dictionaries.TryGetValue(defaultLocale, out DictionaryAggregate? @default))
    {
      Default = @default;
    }
  }

  public string Translate(string key) => Target?.Translate(key) ?? Fallback?.Translate(key) ?? Default?.Translate(key) ?? key;
}
