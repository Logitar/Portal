using Logitar.Portal.Contracts.Users;
using PhoneNumbers;

namespace Logitar.Portal.Domain.Users;

internal static class PhoneNumberExtensions
{
  private const string DefaultRegion = "CA";

  public static bool IsValid(this IPhoneNumber phoneNumber)
  {
    try
    {
      _ = phoneNumber.Parse();

      return true;
    }
    catch (NumberParseException)
    {
      return false;
    }
  }

  public static string FormatToE164(this IPhoneNumber phoneNumber)
  {
    PhoneNumbers.PhoneNumber phone = phoneNumber.Parse();

    return PhoneNumberUtil.GetInstance().Format(phone, PhoneNumberFormat.E164);
  }

  private static PhoneNumbers.PhoneNumber Parse(this IPhoneNumber phoneNumber)
  {
    string phone = string.IsNullOrEmpty(phoneNumber.Extension)
      ? phoneNumber.Number
      : $"{phoneNumber.Number} x{phoneNumber.Extension}";

    return PhoneNumberUtil.GetInstance().Parse(phone.ToString(), phoneNumber.CountryCode ?? DefaultRegion);
  }
}
