using Logitar.Portal.Core.Dictionaries;
using System.Globalization;

namespace Logitar.Portal.Core.Messages;

internal static class MessageHelper
{
  public static Dictionaries GetDictionaries(CultureInfo? locale,
    DictionaryAggregate? defaultDictionary,
    Dictionary<CultureInfo, DictionaryAggregate> dictionaries)
  {
    DictionaryAggregate? preferredDictionary = null;
    DictionaryAggregate? fallbackDictionary = null;

    if (locale != null)
    {
      dictionaries.TryGetValue(locale, out preferredDictionary);

      if (locale.Parent != null)
      {
        dictionaries.TryGetValue(locale.Parent, out fallbackDictionary);
      }
    }

    return new Dictionaries(defaultDictionary, fallbackDictionary, preferredDictionary);
  }
}
