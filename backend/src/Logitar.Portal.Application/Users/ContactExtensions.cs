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

  public static Email ToEmail(this EmailPayload email) => email.ToEmail(email.IsVerified);
  public static Email ToEmail(this IEmail email, bool isVerified = false)
  {
    return new Email(email.Address, isVerified);
  }

  public static Phone ToPhone(this PhonePayload phone) => phone.ToPhone(phone.IsVerified);
  public static Phone ToPhone(this IPhone phone, bool isVerified = false)
  {
    return new Phone(phone.Number, phone.CountryCode, phone.Extension, isVerified);
  }
}
