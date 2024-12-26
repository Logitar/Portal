using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Messages;

public record Dictionaries
{
  public Dictionary? Target { get; }
  public Dictionary? Fallback { get; }
  public Dictionary? Default { get; }

  public Dictionaries()
  {
  }

  public Dictionaries(IReadOnlyDictionary<LocaleUnit, Dictionary> dictionaries, LocaleUnit? targetLocale = null, LocaleUnit? defaultLocale = null) : this()
  {
    if (targetLocale != null)
    {
      if (dictionaries.TryGetValue(targetLocale, out Dictionary? target))
      {
        Target = target;
      }

      if (!string.IsNullOrEmpty(targetLocale.Culture.Parent?.Name))
      {
        LocaleUnit fallbackLocale = new(targetLocale.Culture.Parent.Name);
        if (dictionaries.TryGetValue(fallbackLocale, out Dictionary? fallback))
        {
          Fallback = fallback;
        }
      }
    }

    if (defaultLocale != null && dictionaries.TryGetValue(defaultLocale, out Dictionary? @default))
    {
      Default = @default;
    }
  }

  public string Translate(string key) => Target?.Translate(key) ?? Fallback?.Translate(key) ?? Default?.Translate(key) ?? key;
}
