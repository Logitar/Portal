using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Dictionaries;
using System.Globalization;

namespace Logitar.Portal.Application.Messages
{
  internal static class MessageHelper
  {
    public static Dictionaries GetDictionaries(CultureInfo? locale, Dictionary? defaultDictionary, Dictionary<CultureInfo, Dictionary> dictionaries)
    {
      Dictionary? preferredDictionary = null;
      Dictionary? fallbackDictionary = null;

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

    public static Dictionary<string, string?>? GetVariables(IEnumerable<VariablePayload> payloads)
    {
      Dictionary<string, string?>? variables = payloads?.GroupBy(x => x.Key)
        .ToDictionary(x => x.Key, x => x.FirstOrDefault(y => y.Value != null)?.Value);

      return variables;
    }
  }
}
