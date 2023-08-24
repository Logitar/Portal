using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Application;

internal static class InputExtensions
{
  public static Locale? GetLocale(this string code, string propertyName)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      return null;
    }

    try
    {
      return new Locale(code);
    }
    catch (Exception innerException)
    {
      throw new InvalidLocaleException(code, propertyName, innerException);
    }
  }
  public static Locale GetRequiredLocale(this string code, string propertyName)
  {
    try
    {
      return new Locale(code);
    }
    catch (Exception innerException)
    {
      throw new InvalidLocaleException(code, propertyName, innerException);
    }
  }

  public static ReadOnlyLoggingSettings ToReadOnlyLoggingSettings(this ILoggingSettings input)
    => new(input.Extent, input.OnlyErrors);
  public static ReadOnlyPasswordSettings ToReadOnlyPasswordSettings(this IPasswordSettings input)
    => new(input.RequiredLength, input.RequiredUniqueChars, input.RequireNonAlphanumeric, input.RequireLowercase, input.RequireUppercase, input.RequireDigit);
  public static ReadOnlyUniqueNameSettings ToReadOnlyUniqueNameSettings(this IUniqueNameSettings input)
    => new(input.AllowedCharacters);
}
