using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly IReadOnlyDictionary<ActorId, Actor> _actors;

  public Mapper(IReadOnlyDictionary<ActorId, Actor> actors)
  {
    _actors = actors;
  }

  public Realm Map(RealmEntity source)
  {
    Realm destination = new()
    {
      UniqueSlug = source.UniqueSlug,
      DisplayName = source.DisplayName,
      Description = source.Description,
      DefaultLocale = source.DefaultLocale,
      Secret = source.Secret,
      Url = source.Url,
      RequireUniqueEmail = source.RequireUniqueEmail,
      RequireConfirmedAccount = source.RequireConfirmedAccount,
      UniqueNameSettings = ToUniqueNameSettings(source.UniqueNameSettings),
      PasswordSettings = ToPasswordSettings(source.PasswordSettings),
      ClaimMappings = source.ClaimMappings.Select(claimMapping => new ClaimMapping(claimMapping.Key, claimMapping.Value.Name, claimMapping.Value.Type)),
      CustomAttributes = ToCustomAttributes(source.CustomAttributes)
    };

    MapAggregate(source, destination);

    return destination;
  }

  public User Map(UserEntity source, Realm? realm)
  {
    User destination = new()
    {
      UniqueName = source.UniqueName,
      HasPassword = source.HasPassword,
      PasswordChangedBy = source.PasswordChangedBy == null ? null : GetActor(source.PasswordChangedBy),
      PasswordChangedOn = source.PasswordChangedOn.HasValue ? ToUniversalTime(source.PasswordChangedOn.Value) : null,
      DisabledBy = source.DisabledBy == null ? null : GetActor(source.DisabledBy),
      DisabledOn = source.DisabledOn.HasValue ? ToUniversalTime(source.DisabledOn.Value) : null,
      IsDisabled = source.IsDisabled,
      AuthenticatedOn = source.AuthenticatedOn.HasValue ? ToUniversalTime(source.AuthenticatedOn.Value) : null,
      IsConfirmed = source.IsConfirmed,
      FirstName = source.FirstName,
      MiddleName = source.MiddleName,
      LastName = source.LastName,
      FullName = source.FullName,
      Nickname = source.Nickname,
      Birthdate = source.Birthdate.HasValue ? ToUniversalTime(source.Birthdate.Value) : null,
      Gender = source.Gender,
      Locale = source.Locale,
      TimeZone = source.TimeZone,
      Picture = source.Picture,
      Profile = source.Profile,
      Website = source.Website,
      CustomAttributes = ToCustomAttributes(source.CustomAttributes),
      Realm = realm
    };

    if (source.AddressStreet != null && source.AddressLocality != null
      && source.AddressCountry != null && source.AddressFormatted != null)
    {
      destination.Address = new Address
      {
        Street = source.AddressStreet,
        Locality = source.AddressLocality,
        Region = source.AddressRegion,
        PostalCode = source.AddressPostalCode,
        Country = source.AddressCountry,
        Formatted = source.AddressFormatted,
        IsVerified = source.IsAddressVerified,
        VerifiedBy = source.AddressVerifiedBy == null ? null : GetActor(source.AddressVerifiedBy),
        VerifiedOn = source.AddressVerifiedOn.HasValue ? ToUniversalTime(source.AddressVerifiedOn.Value) : null
      };
    }
    if (source.EmailAddress != null)
    {
      destination.Email = new Email
      {
        Address = source.EmailAddress,
        IsVerified = source.IsEmailVerified,
        VerifiedBy = source.EmailVerifiedBy == null ? null : GetActor(source.EmailVerifiedBy),
        VerifiedOn = source.EmailVerifiedOn.HasValue ? ToUniversalTime(source.EmailVerifiedOn.Value) : null
      };
    }
    if (source.PhoneNumber != null && source.PhoneE164Formatted != null)
    {
      destination.Phone = new Phone
      {
        CountryCode = source.PhoneCountryCode,
        Number = source.PhoneNumber,
        Extension = source.PhoneExtension,
        E164Formatted = source.PhoneE164Formatted,
        IsVerified = source.IsPhoneVerified,
        VerifiedBy = source.PhoneVerifiedBy == null ? null : GetActor(source.PhoneVerifiedBy),
        VerifiedOn = source.PhoneVerifiedOn.HasValue ? ToUniversalTime(source.PhoneVerifiedOn.Value) : null
      };
    }

    // TODO(fpion): Roles

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = source.AggregateId;
    destination.CreatedBy = GetActor(source.CreatedBy);
    destination.CreatedOn = ToUniversalTime(source.CreatedOn);
    destination.UpdatedBy = GetActor(source.UpdatedBy);
    destination.UpdatedOn = ToUniversalTime(source.UpdatedOn);
    destination.Version = source.Version;
  }
  private Actor GetActor(string id) => _actors.TryGetValue(new ActorId(id), out Actor? actor) ? actor : new();

  private static IEnumerable<CustomAttribute> ToCustomAttributes(Dictionary<string, string> customAttributes)
    => customAttributes.Select(customAttribute => new CustomAttribute(customAttribute.Key, customAttribute.Value));

  private static PasswordSettings ToPasswordSettings(ReadOnlyPasswordSettings source) => new()
  {
    RequiredLength = source.RequiredLength,
    RequiredUniqueChars = source.RequiredUniqueChars,
    RequireNonAlphanumeric = source.RequireNonAlphanumeric,
    RequireLowercase = source.RequireLowercase,
    RequireUppercase = source.RequireUppercase,
    RequireDigit = source.RequireDigit
  };

  private static UniqueNameSettings ToUniqueNameSettings(ReadOnlyUniqueNameSettings source) => new()
  {
    AllowedCharacters = source.AllowedCharacters
  };

  private static DateTime ToUniversalTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
}
