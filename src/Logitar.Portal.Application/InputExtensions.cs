using Logitar.EventSourcing;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
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

  public static PostalAddress ToPostalAddress(this AddressPayload address, bool isVerified)
    => new(address.Street, address.Locality, address.Country, address.Region, address.PostalCode, isVerified);
  public static EmailAddress ToEmailAddress(this EmailPayload email, bool isVerified)
    => new(email.Address, isVerified);
  public static PhoneNumber ToPhoneNumber(this PhonePayload phone, bool isVerified)
    => new(phone.Number, phone.CountryCode, phone.Extension, isVerified);

  public static ReadOnlyPasswordSettings ToPasswordSettings(this PasswordSettings passwordSettings)
    => new(passwordSettings.RequiredLength, passwordSettings.RequiredUniqueChars,
      passwordSettings.RequireNonAlphanumeric, passwordSettings.RequireLowercase,
      passwordSettings.RequireUppercase, passwordSettings.RequireDigit);
  public static ReadOnlyUniqueNameSettings ToUniqueNameSettings(this UniqueNameSettings uniqueNameSettings)
    => new(uniqueNameSettings.AllowedCharacters);
}
