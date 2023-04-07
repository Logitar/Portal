using PhoneNumbers;
using System.Text;

namespace Logitar.Portal.v2.Core.Users.Contact;

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
    StringBuilder phone = new();

    if (!string.IsNullOrEmpty(phoneNumber.CountryCode))
    {
      phone.Append(phoneNumber.CountryCode);
      phone.Append(' ');
    }

    phone.Append(phoneNumber.Number);

    if (!string.IsNullOrEmpty(phoneNumber.Extension))
    {
      phone.Append(" x");
      phone.Append(phoneNumber.Extension);
    }

    return PhoneNumberUtil.GetInstance().Parse(phone.ToString(), DefaultRegion);
  }
}
