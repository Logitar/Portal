using AutoMapper;
using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Contracts.Users.Contact;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Profiles;

internal class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<UserEntity, User>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.GetId))
      .ForMember(x => x.PasswordChangedBy, x => x.MapFrom(y => MappingHelper.GetActor(y.PasswordChangedById, y.PasswordChangedBy)))
      .ForMember(x => x.DisabledBy, x => x.MapFrom(y => MappingHelper.GetActor(y.DisabledById, y.DisabledBy)))
      .ForMember(x => x.Address, x => x.MapFrom(GetAddress))
      .ForMember(x => x.Email, x => x.MapFrom(GetEmail))
      .ForMember(x => x.Phone, x => x.MapFrom(GetPhone))
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(MappingHelper.GetCustomAttributes));
  }

  private static Address? GetAddress(UserEntity user, User _)
  {
    if (user.AddressLine1 == null || user.AddressLocality == null
      || user.AddressCountry == null || user.AddressFormatted == null)
    {
      return null;
    }

    return new Address
    {
      Line1 = user.AddressLine1,
      Line2 = user.AddressLine2,
      Locality = user.AddressLocality,
      PostalCode = user.AddressPostalCode,
      Country = user.AddressCountry,
      Region = user.AddressRegion,
      Formatted = user.AddressFormatted,
      VerifiedBy = MappingHelper.GetActor(user.AddressVerifiedById, user.AddressVerifiedBy),
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
      VerifiedBy = MappingHelper.GetActor(user.EmailVerifiedById, user.EmailVerifiedBy),
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
      VerifiedBy = MappingHelper.GetActor(user.PhoneVerifiedById, user.PhoneVerifiedBy),
      VerifiedOn = user.PhoneVerifiedOn,
      IsVerified = user.IsPhoneVerified
    };
  }
}
