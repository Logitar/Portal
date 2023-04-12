using Logitar.Portal.v2.Core.Users;
using System.Globalization;

namespace Logitar.Portal.v2.Core;

internal static class StringExtensions
{
  public static CultureInfo? GetCultureInfo(this string name, string paramName)
  {
    try
    {
      return string.IsNullOrWhiteSpace(name) ? null : CultureInfo.GetCultureInfo(name);
    }
    catch (Exception innerException)
    {
      throw new InvalidLocaleException(name, paramName, innerException);
    }
  }
  public static CultureInfo GetRequiredCultureInfo(this string name, string paramName)
  {
    return name.GetCultureInfo(paramName) ?? throw new InvalidLocaleException(name, paramName);
  }

  public static Gender? GetGender(this string value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value);
  }

  public static TimeZoneEntry? GetTimeZoneEntry(this string id, string paramName)
  {
    try
    {
      return string.IsNullOrWhiteSpace(id) ? null : new(id);
    }
    catch (Exception innerException)
    {
      throw new InvalidTimeZoneEntryException(id, paramName, innerException);
    }
  }

  public static Uri? GetUri(this string url, string paramName)
  {
    try
    {
      return string.IsNullOrWhiteSpace(url) ? null : new(url);
    }
    catch (Exception innerException)
    {
      throw new InvalidUrlException(url, paramName, innerException);
    }
  }
}
