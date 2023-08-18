using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class ReplaceUserCommandHandler : IRequestHandler<ReplaceUserCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ReplaceUserCommandHandler(IApplicationContext applicationContext,
    IPasswordService passwordService, IRealmRepository realmRepository, IUserManager userManager,
    IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(ReplaceUserCommand command, CancellationToken cancellationToken)
  {
    AggregateId userId = command.Id.GetAggregateId(nameof(command.Id));
    UserAggregate? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }

    UserAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _userRepository.LoadAsync(userId, command.Version.Value, cancellationToken);
    }

    ReplaceUserPayload payload = command.Payload;
    bool isAddressVerified = payload.Address?.IsVerified ?? user.Address?.IsVerified ?? false;
    PostalAddress? address = payload.Address?.ToPostalAddress(isAddressVerified);
    bool isEmailVerified = payload.Email?.IsVerified ?? user.Email?.IsVerified ?? false;
    EmailAddress? email = payload.Email?.ToEmailAddress(isEmailVerified);
    bool isPhoneVerified = payload.Phone?.IsVerified ?? user.Phone?.IsVerified ?? false;
    PhoneNumber? phone = payload.Phone?.ToPhoneNumber(isPhoneVerified);
    Gender? gender = payload.Gender?.GetGender(nameof(payload.Gender));
    CultureInfo? locale = payload.Locale?.GetCultureInfo(nameof(payload.Locale));
    TimeZoneEntry? timeZone = payload.TimeZone?.GetTimeZone(nameof(payload.TimeZone));
    Uri? picture = payload.Picture?.GetUrl(nameof(payload.Picture));
    Uri? profile = payload.Profile?.GetUrl(nameof(payload.Profile));
    Uri? website = payload.Website?.GetUrl(nameof(payload.Website));

    if (reference == null || payload.UniqueName.Trim() != reference.UniqueName)
    {
      RealmAggregate? realm = null;
      if (user.TenantId != null)
      {
        AggregateId realmId = new(user.TenantId);
        realm = await _realmRepository.LoadAsync(realmId, cancellationToken)
          ?? throw new InvalidOperationException($"The realm 'Id={realmId}' could not be found.");
      }

      IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;
      user.SetUniqueName(uniqueNameSettings, payload.UniqueName);
    }
    if (payload.Password != null)
    {
      user.SetPassword(_passwordService.Create(payload.Password));
    }
    if (payload.IsDisabled.HasValue)
    {
      if (payload.IsDisabled.Value)
      {
        user.Disable();
      }
      else
      {
        user.Enable();
      }
    }

    if (reference == null || address != reference.Address)
    {
      user.Address = address;
    }
    if (reference == null || email != reference.Email)
    {
      user.Email = email;
    }
    if (reference == null || phone != reference.Phone)
    {
      user.Phone = phone;
    }

    if (reference == null || payload.FirstName?.CleanTrim() != reference.FirstName)
    {
      user.FirstName = payload.FirstName;
    }
    if (reference == null || payload.MiddleName?.CleanTrim() != reference.MiddleName)
    {
      user.MiddleName = payload.MiddleName;
    }
    if (reference == null || payload.LastName?.CleanTrim() != reference.LastName)
    {
      user.LastName = payload.LastName;
    }
    if (reference == null || payload.Nickname?.CleanTrim() != reference.Nickname)
    {
      user.Nickname = payload.Nickname;
    }

    if (reference == null || payload.Birthdate != reference.Birthdate)
    {
      user.Birthdate = payload.Birthdate;
    }
    if (reference == null || gender != reference.Gender)
    {
      user.Gender = gender;
    }
    if (reference == null || locale != reference.Locale)
    {
      user.Locale = locale;
    }
    if (reference == null || timeZone != reference.TimeZone)
    {
      user.TimeZone = timeZone;
    }

    if (reference == null || picture != reference.Picture)
    {
      user.Picture = picture;
    }
    if (reference == null || profile != reference.Profile)
    {
      user.Profile = profile;
    }
    if (reference == null || website != reference.Website)
    {
      user.Website = website;
    }

    HashSet<string> customAttributeKeys = payload.CustomAttributes.Select(x => x.Key).ToHashSet();
    foreach (string customAttributeKey in user.CustomAttributes.Keys)
    {
      if (!customAttributeKeys.Contains(customAttributeKey)
        && (reference == null || reference.CustomAttributes.ContainsKey(customAttributeKey)))
      {
        user.RemoveCustomAttribute(customAttributeKey);
      }
    }
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      user.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    if (user.HasChanges)
    {
      user.Update(_applicationContext.ActorId);

      await _userManager.SaveAsync(user, cancellationToken);
    }

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
