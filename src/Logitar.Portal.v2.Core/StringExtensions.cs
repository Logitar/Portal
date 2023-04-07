using System.Globalization;

namespace Logitar.Portal.v2.Core;

internal static class StringExtensions
{
  public static CultureInfo? GetCultureInfo(this string name, string paramName)
  {
    try
    {
      return CultureInfo.GetCultureInfo(name);
    }
    catch (Exception innerException)
    {
      throw new InvalidLocaleException(name, paramName, innerException);
    }
  }

  public static Uri? GetUri(this string url, string paramName)
  {
    try
    {
      return new Uri(url);
    }
    catch (Exception innerException)
    {
      throw new InvalidUrlException(url, paramName, innerException);
    }
  }
}
