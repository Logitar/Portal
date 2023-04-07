namespace Logitar.Portal.v2.Core;

internal static class DictionaryExtensions
{
  public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
    IEnumerable<KeyValuePair<TKey, TValue>> pairs)
  {
    foreach (KeyValuePair<TKey, TValue> pair in pairs)
    {
      dictionary.Add(pair);
    }
  }
}
