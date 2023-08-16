using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application;

internal static class InputExtensions
{
  public static CultureInfo? GetCultureInfo(this string name, string propertyName)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return null;
    }

    try
    {
      return CultureInfo.GetCultureInfo(name.Trim());
    }
    catch (Exception innerException)
    {
      throw new InvalidLocaleException(name, propertyName, innerException);
    }
  }

  public static ReadOnlyLoggingSettings ToReadOnlyLoggingSettings(this ILoggingSettings loggingSettings) => new()
  {
    Extent = loggingSettings.Extent,
    OnlyErrors = loggingSettings.OnlyErrors
  };
  public static ReadOnlyPasswordSettings ToReadOnlyPasswordSettings(this IPasswordSettings passwordSettings) => new()
  {
    RequiredLength = passwordSettings.RequiredLength,
    RequiredUniqueChars = passwordSettings.RequiredUniqueChars,
    RequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric,
    RequireLowercase = passwordSettings.RequireLowercase,
    RequireUppercase = passwordSettings.RequireUppercase,
    RequireDigit = passwordSettings.RequireDigit,
    Strategy = passwordSettings.Strategy
  };
  public static ReadOnlyUniqueNameSettings ToReadOnlyUniqueNameSettings(this IUniqueNameSettings uniqueNameSettings) => new()
  {
    AllowedCharacters = uniqueNameSettings.AllowedCharacters
  };
}
