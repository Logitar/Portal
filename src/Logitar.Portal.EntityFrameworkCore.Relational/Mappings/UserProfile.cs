using AutoMapper;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Mappings;

internal class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<UserEntity, User>() // TODO(fpion): resolve actors
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.PasswordChangedBy, x => x.Ignore())
      .ForMember(x => x.PasswordChangedOn, x => x.MapFrom(y => MappingHelper.ToUtcDateTime(y.PasswordChangedOn)))
      .ForMember(x => x.DisabledBy, x => x.Ignore())
      .ForMember(x => x.DisabledOn, x => x.MapFrom(y => MappingHelper.ToUtcDateTime(y.DisabledOn)))
      .ForMember(x => x.AuthenticatedOn, x => x.MapFrom(y => MappingHelper.ToUtcDateTime(y.AuthenticatedOn)))
      .ForMember(x => x.Address, x => x.MapFrom(GetAddress))
      .ForMember(x => x.Email, x => x.MapFrom(GetEmail))
      .ForMember(x => x.Phone, x => x.MapFrom(GetPhone))
      .ForMember(x => x.Birthdate, x => x.MapFrom(y => MappingHelper.ToUtcDateTime(y.Birthdate)))
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(y => MappingHelper.GetCustomAttributes(y.CustomAttributes)));
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
      VerifiedBy = null, // TODO(fpion): resolve actor
      VerifiedOn = MappingHelper.ToUtcDateTime(user.AddressVerifiedOn),
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
      VerifiedBy = null, // TODO(fpion): resolve actor
      VerifiedOn = MappingHelper.ToUtcDateTime(user.EmailVerifiedOn),
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
      VerifiedBy = null, // TODO(fpion): resolve actor
      VerifiedOn = MappingHelper.ToUtcDateTime(user.PhoneVerifiedOn),
      IsVerified = user.IsPhoneVerified
    };
  }
}
