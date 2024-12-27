using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly Dictionary<ActorId, ActorModel> _actors = [];

  public Mapper()
  {
  }

  public Mapper(IEnumerable<ActorModel> actors)
  {
    foreach (ActorModel actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public static ActorModel ToActor(ActorEntity actor) => new(actor.DisplayName)
  {
    Id = new ActorId(actor.Id).ToGuid(),
    Type = Enum.Parse<ActorType>(actor.Type),
    IsDeleted = actor.IsDeleted,
    EmailAddress = actor.EmailAddress,
    PictureUrl = actor.PictureUrl
  };

  public ApiKeyModel ToApiKey(ApiKeyEntity source, RealmModel? realm)
  {
    ApiKeyModel destination = new(source.DisplayName)
    {
      Description = source.Description,
      ExpiresOn = AsUniversalTime(source.ExpiresOn),
      AuthenticatedOn = AsUniversalTime(source.AuthenticatedOn),
      Realm = realm
    };

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    foreach (RoleEntity role in source.Roles)
    {
      destination.Roles.Add(ToRole(role, realm));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public ConfigurationModel ToConfiguration(Configuration source)
  {
    ConfigurationModel destination = new(source.Secret.Value)
    {
      DefaultLocale = LocaleModel.TryCreate(source.DefaultLocale?.Code),
      UniqueNameSettings = new UniqueNameSettings(source.UniqueNameSettings),
      PasswordSettings = new PasswordSettings(source.PasswordSettings),
      RequireUniqueEmail = source.RequireUniqueEmail,
      LoggingSettings = new LoggingSettings(source.LoggingSettings)
    };

    MapAggregate(source, destination);

    return destination;
  }

  public DictionaryModel ToDictionary(DictionaryEntity source, RealmModel? realm)
  {
    DictionaryModel destination = new(new LocaleModel(source.Locale))
    {
      EntryCount = source.EntryCount,
      Realm = realm
    };

    foreach (KeyValuePair<string, string> entry in source.GetEntries())
    {
      destination.Entries.Add(new DictionaryEntry(entry));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public MessageModel ToMessage(MessageEntity source, RealmModel? realm, IEnumerable<UserEntity> users)
  {
    ContentModel body = new(source.BodyType, source.BodyText);

    SenderModel sender;
    if (source.Sender == null)
    {
      sender = new()
      {
        IsDefault = source.SenderIsDefault,
        EmailAddress = source.SenderAddress,
        PhoneNumber = source.SenderPhoneNumber,
        DisplayName = source.SenderDisplayName,
        Provider = source.SenderProvider
      };
    }
    else
    {
      sender = ToSender(source.Sender, realm);
    }

    TemplateModel template;
    if (source.Template == null)
    {
      template = new()
      {
        UniqueKey = source.TemplateUniqueKey,
        DisplayName = source.TemplateDisplayName
      };
    }
    else
    {
      template = ToTemplate(source.Template, realm);
    }

    MessageModel destination = new(source.Subject, body, sender, template)
    {
      RecipientCount = source.RecipientCount,
      IgnoreUserLocale = source.IgnoreUserLocale,
      Locale = LocaleModel.TryCreate(source.Locale),
      IsDemo = source.IsDemo,
      Status = source.Status,
      Realm = realm
    };

    Dictionary<int, UserModel> usersById = new(capacity: users.Count());
    foreach (UserEntity user in users)
    {
      usersById[user.UserId] = ToUser(user, realm);
    }
    foreach (RecipientEntity recipient in source.Recipients)
    {
      UserModel? user = null;
      if (recipient.UserId.HasValue)
      {
        _ = usersById.TryGetValue(recipient.UserId.Value, out user);
      }

      destination.Recipients.Add(ToRecipient(recipient, user));
    }

    foreach (KeyValuePair<string, string> variable in source.GetVariables())
    {
      destination.Variables.Add(new Variable(variable));
    }

    foreach (KeyValuePair<string, string> data in source.GetResultData())
    {
      destination.ResultData.Add(new ResultData(data));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public OneTimePasswordModel ToOneTimePassword(OneTimePasswordEntity source, RealmModel? realm)
  {
    OneTimePasswordModel destination = new()
    {
      ExpiresOn = AsUniversalTime(source.ExpiresOn),
      MaximumAttempts = source.MaximumAttempts,
      AttemptCount = source.AttemptCount,
      HasValidationSucceeded = source.HasValidationSucceeded,
      Realm = realm
    };

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public RealmModel ToRealm(RealmEntity source)
  {
    RealmModel destination = new(source.UniqueSlug, source.Secret)
    {
      DisplayName = source.DisplayName,
      Description = source.Description,
      DefaultLocale = LocaleModel.TryCreate(source.DefaultLocale),
      Url = source.Url,
      UniqueNameSettings = new UniqueNameSettings(source.AllowedUniqueNameCharacters),
      PasswordSettings = new PasswordSettings(source.RequiredPasswordLength, source.RequiredPasswordUniqueChars, source.PasswordsRequireNonAlphanumeric,
        source.PasswordsRequireLowercase, source.PasswordsRequireUppercase, source.PasswordsRequireDigit, source.PasswordHashingStrategy),
      RequireUniqueEmail = source.RequireUniqueEmail
    };

    foreach (KeyValuePair<string, string> customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public static RecipientModel ToRecipient(RecipientEntity source, UserModel? user)
  {
    RecipientModel destination = new()
    {
      Type = source.Type,
      Address = source.Address,
      DisplayName = source.DisplayName,
      PhoneNumber = source.PhoneNumber
    };

    if (user != null)
    {
      destination.User = user;
    }
    else if (source.UserUniqueName != null)
    {
      destination.User = new(source.UserUniqueName)
      {
        FullName = source.UserFullName,
        Picture = source.UserPicture
      };
      if (source.UserEmailAddress != null)
      {
        destination.User.Email = new EmailModel(source.UserEmailAddress);
      }
    }

    return destination;
  }

  public RoleModel ToRole(RoleEntity source, RealmModel? realm)
  {
    RoleModel destination = new(source.UniqueName)
    {
      DisplayName = source.DisplayName,
      Description = source.Description,
      Realm = realm
    };

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public SessionModel ToSession(SessionEntity source, RealmModel? realm)
  {
    if (source.User == null)
    {
      throw new ArgumentException($"The {nameof(source.User)} is required.", nameof(source));
    }

    UserModel user = ToUser(source.User, realm);
    SessionModel destination = new(user)
    {
      IsPersistent = source.IsPersistent,
      IsActive = source.IsActive,
      SignedOutBy = TryFindActor(source.SignedOutBy),
      SignedOutOn = AsUniversalTime(source.SignedOutOn)
    };

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public SenderModel ToSender(SenderEntity source, RealmModel? realm)
  {
    SenderModel destination = new()
    {
      IsDefault = source.IsDefault,
      EmailAddress = source.EmailAddress,
      PhoneNumber = source.PhoneNumber,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Provider = source.Provider,
      Realm = realm
    };

    Dictionary<string, string> settings = source.GetSettings();
    switch (source.Provider)
    {
      case SenderProvider.Mailgun:
        destination.Mailgun = new MailgunSettings(settings[nameof(IMailgunSettings.ApiKey)], settings[nameof(IMailgunSettings.DomainName)]);
        break;
      case SenderProvider.SendGrid:
        destination.SendGrid = new SendGridSettings(settings[nameof(ISendGridSettings.ApiKey)]);
        break;
      case SenderProvider.Twilio:
        destination.Twilio = new TwilioSettings(settings[nameof(ITwilioSettings.AccountSid)], settings[nameof(ITwilioSettings.AuthenticationToken)]);
        break;
      default:
        throw new SenderProviderNotSupportedException(source.Provider);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public TemplateModel ToTemplate(TemplateEntity source, RealmModel? realm)
  {
    ContentModel content = new(source.ContentType, source.ContentText);
    TemplateModel destination = new(source.UniqueKey, source.Subject, content)
    {
      DisplayName = source.DisplayName,
      Description = source.Description,
      Realm = realm
    };

    MapAggregate(source, destination);

    return destination;
  }

  public UserModel ToUser(UserEntity source, RealmModel? realm)
  {
    UserModel destination = new(source.UniqueName)
    {
      HasPassword = source.HasPassword,
      PasswordChangedBy = TryFindActor(source.PasswordChangedBy),
      PasswordChangedOn = AsUniversalTime(source.PasswordChangedOn),
      DisabledBy = TryFindActor(source.DisabledBy),
      DisabledOn = AsUniversalTime(source.DisabledOn),
      IsDisabled = source.IsDisabled,
      IsConfirmed = source.IsConfirmed,
      FirstName = source.FirstName,
      MiddleName = source.MiddleName,
      LastName = source.LastName,
      FullName = source.FullName,
      Nickname = source.Nickname,
      Birthdate = AsUniversalTime(source.Birthdate),
      Gender = source.Gender,
      Locale = LocaleModel.TryCreate(source.Locale),
      TimeZone = source.TimeZone,
      Picture = source.Picture,
      Profile = source.Profile,
      Website = source.Website,
      AuthenticatedOn = AsUniversalTime(source.AuthenticatedOn),
      Realm = realm
    };

    if (source.AddressStreet != null && source.AddressLocality != null && source.AddressCountry != null && source.AddressFormatted != null)
    {
      destination.Address = new AddressModel(source.AddressStreet, source.AddressLocality, source.AddressPostalCode, source.AddressRegion, source.AddressCountry, source.AddressFormatted)
      {
        IsVerified = source.IsAddressVerified,
        VerifiedBy = TryFindActor(source.AddressVerifiedBy),
        VerifiedOn = AsUniversalTime(source.AddressVerifiedOn)
      };
    }
    if (source.EmailAddress != null)
    {
      destination.Email = new EmailModel(source.EmailAddress)
      {
        IsVerified = source.IsEmailVerified,
        VerifiedBy = TryFindActor(source.EmailVerifiedBy),
        VerifiedOn = AsUniversalTime(source.EmailVerifiedOn)
      };
    }
    if (source.PhoneNumber != null && source.PhoneE164Formatted != null)
    {
      destination.Phone = new PhoneModel(source.PhoneCountryCode, source.PhoneNumber, source.PhoneExtension, source.PhoneE164Formatted)
      {
        IsVerified = source.IsPhoneVerified,
        VerifiedBy = TryFindActor(source.PhoneVerifiedBy),
        VerifiedOn = AsUniversalTime(source.PhoneVerifiedOn)
      };
    }

    foreach (KeyValuePair<string, string> customAttribute in source.CustomAttributes)
    {
      destination.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    foreach (IdentifierEntity identifier in source.Identifiers)
    {
      destination.CustomIdentifiers.Add(new CustomIdentifier(identifier.Key, identifier.Value));
    }

    foreach (RoleEntity role in source.Roles)
    {
      destination.Roles.Add(ToRole(role, realm));
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateRoot source, AggregateModel destination)
  {
    destination.Id = source.Id.ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = AsUniversalTime(source.CreatedOn);
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedOn);
  }
  private void MapAggregate(AggregateEntity source, AggregateModel destination)
  {
    destination.Id = new AggregateId(source.AggregateId).ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = AsUniversalTime(source.CreatedOn);
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedOn);
  }

  private ActorModel? TryFindActor(string? id) => id == null ? null : FindActor(id);
  private ActorModel FindActor(string id) => FindActor(new ActorId(id));
  private ActorModel FindActor(ActorId id) => _actors.TryGetValue(id, out ActorModel? actor) ? actor : ActorModel.System;

  private static DateTime? AsUniversalTime(DateTime? value) => value.HasValue ? AsUniversalTime(value.Value) : null;
  private static DateTime AsUniversalTime(DateTime value) => value.Kind switch
  {
    DateTimeKind.Local => value.ToUniversalTime(),
    DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    DateTimeKind.Utc => value,
    _ => throw new NotSupportedException($"The date time kind '{value.Kind}' is not supported."),
  };
}
