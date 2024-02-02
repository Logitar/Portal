using Logitar.Identity.Contracts.Users;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

internal static class ContactExtensions
{
  public static AddressUnit ToAddressUnit(this AddressPayload address) => address.ToAddressUnit(address.IsVerified);
  public static AddressUnit ToAddressUnit(this IAddress address, bool isVerified = false)
  {
    return new AddressUnit(address.Street, address.Locality, address.Country, address.Region, address.PostalCode, isVerified);
  }

  public static EmailUnit ToEmailUnit(this EmailPayload email) => email.ToEmailUnit(email.IsVerified);
  public static EmailUnit ToEmailUnit(this IEmail email, bool isVerified = false)
  {
    return new EmailUnit(email.Address, isVerified);
  }

  public static PhoneUnit ToPhoneUnit(this PhonePayload phone) => phone.ToPhoneUnit(phone.IsVerified);
  public static PhoneUnit ToPhoneUnit(this IPhone phone, bool isVerified = false)
  {
    return new PhoneUnit(phone.Number, phone.CountryCode, phone.Extension, isVerified);
  }
}
