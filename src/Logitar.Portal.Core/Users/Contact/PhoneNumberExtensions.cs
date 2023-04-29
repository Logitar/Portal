using PhoneNumbers;

namespace Logitar.Portal.Core.Users.Contact;

public static class PhoneNumberExtensions
{
  private const string DefaultRegion = "US";

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

  public static string ToE164String(this IPhoneNumber phoneNumber)
  {
    PhoneNumber phone = phoneNumber.Parse();

    return PhoneNumberUtil.GetInstance().Format(phone, PhoneNumberFormat.E164);
  }

  private static PhoneNumber Parse(this IPhoneNumber phoneNumber)
  {
    string phone = string.IsNullOrEmpty(phoneNumber.Extension)
      ? phoneNumber.Number
      : $"{phoneNumber.Number} x{phoneNumber.Extension}";

    return PhoneNumberUtil.GetInstance().Parse(phone.ToString(), phoneNumber.CountryCode ?? DefaultRegion);
  }
}
