using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application;

internal static class InputExtensions
{
  public static AggregateId GetAggregateId(this string value, string propertyName)
  {
    try
    {
      return new AggregateId(value);
    }
    catch (Exception innerException)
    {
      throw new InvalidAggregateIdException(value, propertyName, innerException);
    }
  }

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

  public static Uri? GetUrl(this string uriString, string propertyName)
  {
    if (string.IsNullOrWhiteSpace(uriString))
    {
      return null;
    }

    try
    {
      return new Uri(uriString.Trim());
    }
    catch (Exception innerException)
    {
      throw new InvalidUrlException(uriString, propertyName, innerException);
    }
  }

  public static ReadOnlyLoggingSettings ToReadOnlyLoggingSettings(this LoggingSettings loggingSettings) => new()
  {
    Extent = loggingSettings.Extent,
    OnlyErrors = loggingSettings.OnlyErrors
  };
  public static ReadOnlyPasswordSettings ToReadOnlyPasswordSettings(this PasswordSettings passwordSettings) => new()
  {
    RequiredLength = passwordSettings.RequiredLength,
    RequiredUniqueChars = passwordSettings.RequiredUniqueChars,
    RequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric,
    RequireLowercase = passwordSettings.RequireLowercase,
    RequireUppercase = passwordSettings.RequireUppercase,
    RequireDigit = passwordSettings.RequireDigit
  };
  public static ReadOnlyUniqueNameSettings ToReadOnlyUniqueNameSettings(this UniqueNameSettings uniqueNameSettings) => new()
  {
    AllowedCharacters = uniqueNameSettings.AllowedCharacters
  };
}
