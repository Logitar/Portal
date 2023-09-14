using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors;

  public Mapper(Dictionary<ActorId, Actor> actors)
  {
    _actors = actors;
  }

  public ApiKey ToApiKey(ApiKeyEntity source, Realm? realm)
  {
    ApiKey destination = new()
    {
      Id = new AggregateId(source.AggregateId).ToGuid(),
      DisplayName = source.Title,
      Description = source.Description,
      ExpiresOn = source.ExpiresOn,
      AuthenticatedOn = source.AuthenticatedOn,
      CustomAttributes = ToCustomAttributes(source.CustomAttributes),
      Roles = source.Roles.Select(role => ToRole(role, realm)),
      Realm = realm
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Configuration ToConfiguration(ConfigurationAggregate source)
  {
    Configuration destination = new()
    {
      DefaultLocale = source.DefaultLocale.Code,
      Secret = source.Secret,
      UniqueNameSettings = new UniqueNameSettings
      {
        AllowedCharacters = source.UniqueNameSettings.AllowedCharacters
      },
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = source.PasswordSettings.RequiredLength,
        RequiredUniqueChars = source.PasswordSettings.RequiredUniqueChars,
        RequireNonAlphanumeric = source.PasswordSettings.RequireNonAlphanumeric,
        RequireLowercase = source.PasswordSettings.RequireLowercase,
        RequireUppercase = source.PasswordSettings.RequireUppercase,
        RequireDigit = source.PasswordSettings.RequireDigit,
        Strategy = source.PasswordSettings.Strategy
      },
      LoggingSettings = new LoggingSettings
      {
        Extent = source.LoggingSettings.Extent,
        OnlyErrors = source.LoggingSettings.OnlyErrors
      }
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Dictionary ToDictionary(DictionaryEntity source, Realm? realm)
  {
    Dictionary destination = new()
    {
      Id = new AggregateId(source.AggregateId).ToGuid(),
      Realm = realm,
      Locale = source.Locale,
      Entries = source.Entries.Select(entry => new DictionaryEntry(entry.Key, entry.Value))
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Identifier ToIdentifier(IdentifierEntity source)
  {
    return new Identifier()
    {
      Id = source.Id,
      Key = source.Key,
      Value = source.Value,
      CreatedBy = FindActor(source.CreatedBy),
      CreatedOn = ToUniversalTime(source.CreatedOn),
      UpdatedBy = FindActor(source.UpdatedBy),
      UpdatedOn = ToUniversalTime(source.UpdatedOn),
      Version = source.Version
    };
  }

  public Realm ToRealm(RealmEntity source)
  {
    Realm destination = new()
    {
      Id = new AggregateId(source.AggregateId).ToGuid(),
      UniqueSlug = source.UniqueSlug,
      DisplayName = source.DisplayName,
      Description = source.Description,
      DefaultLocale = source.DefaultLocale,
      Secret = source.Secret,
      Url = source.Url,
      RequireUniqueEmail = source.RequireUniqueEmail,
      RequireConfirmedAccount = source.RequireConfirmedAccount,
      UniqueNameSettings = new UniqueNameSettings
      {
        AllowedCharacters = source.AllowedUniqueNameCharacters
      },
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = source.RequiredPasswordLength,
        RequiredUniqueChars = source.RequiredPasswordUniqueChars,
        RequireNonAlphanumeric = source.PasswordsRequireNonAlphanumeric,
        RequireLowercase = source.PasswordsRequireLowercase,
        RequireUppercase = source.PasswordsRequireUppercase,
        RequireDigit = source.PasswordsRequireDigit,
        Strategy = source.PasswordStrategy
      },
      ClaimMappings = source.ClaimMappings.Select(claimMapping => new ClaimMapping(claimMapping.Key, claimMapping.Value.Name, claimMapping.Value.Type)),
      CustomAttributes = ToCustomAttributes(source.CustomAttributes)
    };

    MapAggregate(source, destination);

    if (source.PasswordRecoverySender != null)
    {
      destination.PasswordRecoverySender = ToSender(source.PasswordRecoverySender, realm: null);
    }
    if (source.PasswordRecoveryTemplate != null)
    {
      destination.PasswordRecoveryTemplate = ToTemplate(source.PasswordRecoveryTemplate, realm: null);
    }

    return destination;
  }

  public Role ToRole(RoleEntity source, Realm? realm)
  {
    Role destination = new()
    {
      Id = new AggregateId(source.AggregateId).ToGuid(),
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      CustomAttributes = ToCustomAttributes(source.CustomAttributes),
      Realm = realm
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Sender ToSender(SenderEntity source, Realm? realm)
  {
    Sender destination = new()
    {
      Id = new AggregateId(source.AggregateId).ToGuid(),
      IsDefault = source.IsDefault,
      EmailAddress = source.EmailAddress,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Provider = source.Provider,
      Settings = source.Settings.Select(setting => new ProviderSetting(setting.Key, setting.Value)),
      Realm = realm
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Session ToSession(SessionEntity source, Realm? realm)
  {
    Session destination = new()
    {
      Id = new AggregateId(source.AggregateId).ToGuid(),
      IsPersistent = source.IsPersistent,
      IsActive = source.IsActive,
      SignedOutBy = source.SignedOutBy == null ? null : FindActor(source.SignedOutBy),
      SignedOutOn = source.SignedOutOn.HasValue ? ToUniversalTime(source.SignedOutOn.Value) : null,
      CustomAttributes = ToCustomAttributes(source.CustomAttributes)
    };
    if (source.User != null)
    {
      destination.User = ToUser(source.User, realm);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public Template ToTemplate(TemplateEntity source, Realm? realm)
  {
    Template destination = new()
    {
      Id = new AggregateId(source.AggregateId).ToGuid(),
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Subject = source.Subject,
      ContentType = source.ContentType,
      Contents = source.Contents,
      Realm = realm
    };

    MapAggregate(source, destination);

    return destination;
  }

  public User ToUser(UserEntity source, Realm? realm)
  {
    User destination = new()
    {
      Id = new AggregateId(source.AggregateId).ToGuid(),
      UniqueName = source.UniqueName,
      HasPassword = source.HasPassword,
      PasswordChangedBy = source.PasswordChangedBy == null ? null : FindActor(source.PasswordChangedBy),
      PasswordChangedOn = source.PasswordChangedOn.HasValue ? ToUniversalTime(source.PasswordChangedOn.Value) : null,
      DisabledBy = source.DisabledBy == null ? null : FindActor(source.DisabledBy),
      DisabledOn = source.DisabledOn.HasValue ? ToUniversalTime(source.DisabledOn.Value) : null,
      IsDisabled = source.IsDisabled,
      AuthenticatedOn = source.AuthenticatedOn.HasValue ? ToUniversalTime(source.AuthenticatedOn.Value) : null,
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
      Identifiers = source.Identifiers.Select(ToIdentifier),
      Roles = source.Roles.Select(role => ToRole(role, realm)),
      Realm = realm
    };

    if (source.AddressStreet != null && source.AddressLocality != null && source.AddressCountry != null && source.AddressFormatted != null)
    {
      destination.Address = new Address
      {
        Street = source.AddressStreet,
        Locality = source.AddressLocality,
        Region = source.AddressRegion,
        PostalCode = source.AddressPostalCode,
        Country = source.AddressCountry,
        Formatted = source.AddressFormatted,
        VerifiedBy = source.AddressVerifiedBy == null ? null : FindActor(source.AddressVerifiedBy),
        VerifiedOn = source.AddressVerifiedOn.HasValue ? ToUniversalTime(source.AddressVerifiedOn.Value) : null,
        IsVerified = source.IsAddressVerified
      };
    }

    if (source.EmailAddress != null)
    {
      destination.Email = new Email(source.EmailAddress)
      {
        VerifiedBy = source.EmailVerifiedBy == null ? null : FindActor(source.EmailVerifiedBy),
        VerifiedOn = source.EmailVerifiedOn.HasValue ? ToUniversalTime(source.EmailVerifiedOn.Value) : null,
        IsVerified = source.IsEmailVerified
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
        VerifiedBy = source.PhoneVerifiedBy == null ? null : FindActor(source.PhoneVerifiedBy),
        VerifiedOn = source.PhoneVerifiedOn.HasValue ? ToUniversalTime(source.PhoneVerifiedOn.Value) : null,
        IsVerified = source.IsPhoneVerified
      };
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateRoot source, Aggregate destination)
  {
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.ToUniversalTime();
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.ToUniversalTime();
    destination.Version = source.Version;
  }
  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = ToUniversalTime(source.CreatedOn);
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = ToUniversalTime(source.UpdatedOn);
    destination.Version = source.Version;
  }
  private Actor FindActor(string id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : new();

  private static IEnumerable<CustomAttribute> ToCustomAttributes(Dictionary<string, string> customAttributes)
    => customAttributes.Select(customAttribute => new CustomAttribute(customAttribute.Key, customAttribute.Value));
  private static DateTime ToUniversalTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
}
