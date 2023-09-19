using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application;

internal static class InputExtensions
{
  public static Gender? GetGender(this string value, string propertyName)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    try
    {
      return new Gender(value);
    }
    catch (Exception innerException)
    {
      throw new InvalidGenderException(value, propertyName, innerException);
    }
  }

  public static ReadOnlyLocale? GetLocale(this string code, string propertyName)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      return null;
    }

    try
    {
      return new ReadOnlyLocale(code);
    }
    catch (Exception innerException)
    {
      throw new InvalidLocaleException(code, propertyName, innerException);
    }
  }
  public static ReadOnlyLocale GetRequiredLocale(this string code, string propertyName)
  {
    try
    {
      return new ReadOnlyLocale(code);
    }
    catch (Exception innerException)
    {
      throw new InvalidLocaleException(code, propertyName, innerException);
    }
  }

  public static TimeZoneEntry? GetTimeZone(this string id, string propertyName)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      return null;
    }

    try
    {
      return new TimeZoneEntry(id);
    }
    catch (Exception innerException)
    {
      throw new InvalidTimeZoneEntryException(id, propertyName, innerException);
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

  public static EmailAddress ToEmailAddress(this EmailPayload payload)
    => new(payload.Address, payload.IsVerified);
  public static PhoneNumber ToPhoneNumber(this PhonePayload payload)
    => new(payload.Number, payload.CountryCode, payload.Extension, payload.IsVerified);
  public static PostalAddress ToPostalAddress(this AddressPayload payload)
    => new(payload.Street, payload.Locality, payload.Country, payload.Region, payload.PostalCode, payload.IsVerified);

  public static ReadOnlyLoggingSettings ToReadOnlyLoggingSettings(this ILoggingSettings input)
    => new(input.Extent, input.OnlyErrors);
  public static ReadOnlyPasswordSettings ToReadOnlyPasswordSettings(this IPasswordSettings input)
    => new(input.RequiredLength, input.RequiredUniqueChars, input.RequireNonAlphanumeric,
        input.RequireLowercase, input.RequireUppercase, input.RequireDigit, input.Strategy);
  public static ReadOnlyUniqueNameSettings ToReadOnlyUniqueNameSettings(this IUniqueNameSettings input)
    => new(input.AllowedCharacters);
}
