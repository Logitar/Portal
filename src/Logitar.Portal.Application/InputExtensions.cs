using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain;

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

  public static ReadOnlyPasswordSettings ToPasswordSettings(this PasswordSettings passwordSettings)
    => new(passwordSettings.RequiredLength, passwordSettings.RequiredUniqueChars,
      passwordSettings.RequireNonAlphanumeric, passwordSettings.RequireLowercase,
      passwordSettings.RequireUppercase, passwordSettings.RequireDigit);
  public static ReadOnlyUniqueNameSettings ToUniqueNameSettings(this UniqueNameSettings uniqueNameSettings)
    => new(uniqueNameSettings.AllowedCharacters);
}
