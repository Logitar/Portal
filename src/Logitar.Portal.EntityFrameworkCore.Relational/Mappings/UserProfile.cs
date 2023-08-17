using AutoMapper;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Mappings;

internal class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<UserEntity, User>() // TODO(fpion): resolve actors
      .ForMember(x => x.PasswordChangedBy, x => x.Ignore())
      .ForMember(x => x.DisabledBy, x => x.Ignore())
      .ForMember(x => x.Address, x => x.MapFrom(GetAddress))
      .ForMember(x => x.Email, x => x.MapFrom(GetEmail))
      .ForMember(x => x.Phone, x => x.MapFrom(GetPhone));
  }

  private static Address? GetAddress(UserEntity user, User _)
  {
    if (user.AddressStreet == null || user.AddressLocality == null
      || user.AddressCountry == null || user.AddressFormatted == null)
    {
      return null;
    }

    return new Address
    {
      Street = user.AddressStreet,
      Locality = user.AddressLocality,
      Region = user.AddressRegion,
      PostalCode = user.AddressPostalCode,
      Country = user.AddressCountry,
      Formatted = user.AddressFormatted,
      VerifiedBy = user.AddressVerifiedBy == null ? null : new Actor(), // TODO(fpion): resolve actor
      VerifiedOn = user.AddressVerifiedOn,
      IsVerified = user.IsAddressVerified
    };
  }
  private static Email? GetEmail(UserEntity user, User _)
  {
    if (user.EmailAddress == null)
    {
      return null;
    }

    return new Email
    {
      Address = user.EmailAddress,
      VerifiedBy = user.EmailVerifiedBy == null ? null : new Actor(), // TODO(fpion): resolve actor
      VerifiedOn = user.EmailVerifiedOn,
      IsVerified = user.IsEmailVerified
    };
  }
  private static Phone? GetPhone(UserEntity user, User _)
  {
    if (user.PhoneNumber == null || user.PhoneE164Formatted == null)
    {
      return null;
    }

    return new Phone
    {
      CountryCode = user.PhoneCountryCode,
      Number = user.PhoneNumber,
      Extension = user.PhoneExtension,
      E164Formatted = user.PhoneE164Formatted,
      VerifiedBy = user.PhoneVerifiedBy == null ? null : new Actor(), // TODO(fpion): resolve actor
      VerifiedOn = user.PhoneVerifiedOn,
      IsVerified = user.IsPhoneVerified
    };
  }
}
