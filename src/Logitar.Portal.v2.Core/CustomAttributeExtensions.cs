using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Core;

internal static class CustomAttributeExtensions
{
  public static Dictionary<string, string> CleanTrim(this Dictionary<string, string> customAttributes)
  {
    return customAttributes.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
      .GroupBy(x => x.Key.Trim())
      .ToDictionary(x => x.Key, x => x.Last().Value.Trim());
  }

  public static Dictionary<string, string> ToDictionary(this IEnumerable<CustomAttribute> customAttributes)
  {
    return customAttributes.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
      .GroupBy(x => x.Key.Trim())
      .ToDictionary(x => x.Key, x => x.Last().Value);
  }
}
