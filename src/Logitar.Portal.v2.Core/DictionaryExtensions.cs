﻿using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Dictionaries;
using Logitar.Portal.v2.Contracts.Senders;

namespace Logitar.Portal.v2.Core;

internal static class DictionaryExtensions
{
  public static Dictionary<string, string> CleanTrim(this Dictionary<string, string> customAttributes)
  {
    return customAttributes.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
      .GroupBy(x => x.Key.Trim())
      .ToDictionary(x => x.Key, x => x.Last().Value.Trim());
  }

  public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
    IEnumerable<KeyValuePair<TKey, TValue>> pairs)
  {
    foreach (KeyValuePair<TKey, TValue> pair in pairs)
    {
      dictionary.Add(pair);
    }
  }

  public static Dictionary<string, string> ToDictionary(this IEnumerable<CustomAttribute> customAttributes)
  {
    return customAttributes.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
      .GroupBy(x => x.Key.Trim())
      .ToDictionary(x => x.Key, x => x.Last().Value);
  }
  public static Dictionary<string, string> ToDictionary(this IEnumerable<Entry> entries)
  {
    return entries.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
      .GroupBy(x => x.Key.Trim())
      .ToDictionary(x => x.Key, x => x.Last().Value);
  }
  public static Dictionary<string, string> ToDictionary(this IEnumerable<Setting> settings)
  {
    return settings.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
      .GroupBy(x => x.Key.Trim())
      .ToDictionary(x => x.Key, x => x.Last().Value);
  }
}
