using Logitar.EventSourcing;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class ReplaceUserCommandHandler : IRequestHandler<ReplaceUserCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ReplaceUserCommandHandler(IApplicationContext applicationContext, IMediator mediator, IPasswordService passwordService,
    IRealmRepository realmRepository, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _mediator = mediator;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(ReplaceUserCommand command, CancellationToken cancellationToken)
  {
    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null)
    {
      return null;
    }

    RealmAggregate? realm = null;
    if (user.TenantId != null)
    {
      realm = await _realmRepository.LoadAsync(user, cancellationToken);
    }

    UserAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _userRepository.LoadAsync(user.Id, command.Version.Value, cancellationToken);
    }

    ReplaceUserPayload payload = command.Payload;

    await SetAuthenticationInformationAsync(user, reference, payload, realm, cancellationToken);
    await SetContactInformationAsync(user, reference, payload, realm, cancellationToken);
    SetPersonalInformation(user, reference, payload);
    SetCustomAttributes(user, reference, payload);
    await SetRolesAsync(user, reference, payload, realm, cancellationToken);

    user.Update(_applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }

  private async Task SetAuthenticationInformationAsync(UserAggregate user, UserAggregate? reference,
    ReplaceUserPayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.Id.Value;

    if (reference == null || payload.UniqueName.Trim() != reference.UniqueName)
    {
      UserAggregate? other = await _userRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken);
      if (other?.Equals(user) == false)
      {
        throw new UniqueNameAlreadyUsedException<UserAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
      }

      IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;
      user.SetUniqueName(uniqueNameSettings, payload.UniqueName);
    }
    if (payload.Password != null)
    {
      IPasswordSettings passwordSettings = realm?.PasswordSettings ?? _applicationContext.Configuration.PasswordSettings;
      user.SetPassword(_passwordService.Create(passwordSettings, payload.Password));
    }

    if (payload.IsDisabled)
    {
      user.Disable(_applicationContext.ActorId);
    }
    else
    {
      user.Enable(_applicationContext.ActorId);
    }
  }

  private async Task SetContactInformationAsync(UserAggregate user, UserAggregate? reference,
    ReplaceUserPayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.Id.Value;
    PostalAddress? address = payload.Address?.ToPostalAddress();
    EmailAddress? email = payload.Email?.ToEmailAddress();
    PhoneNumber? phone = payload.Phone?.ToPhoneNumber();

    if (reference == null || address != reference.Address)
    {
      user.Address = address;
    }
    if (reference == null || email != reference.Email)
    {
      if (email != null && realm?.RequireUniqueEmail == true)
      {
        IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(tenantId, email, cancellationToken);
        if (users.Any(u => !u.Equals(user)))
        {
          throw new EmailAddressAlreadyUsedException(tenantId, email, nameof(payload.Email));
        }
      }

      user.Email = email;
    }
    if (reference == null || phone != reference.Phone)
    {
      user.Phone = phone;
    }
  }

  private static void SetPersonalInformation(UserAggregate user, UserAggregate? reference, ReplaceUserPayload payload)
  {
    Gender? gender = payload.Gender?.GetGender(nameof(payload.Gender));
    Locale? locale = payload.Locale?.GetLocale(nameof(payload.Locale));
    TimeZoneEntry? timeZone = payload.TimeZone?.GetTimeZone(nameof(payload.TimeZone));
    Uri? picture = payload.Picture?.GetUrl(nameof(payload.Picture));
    Uri? profile = payload.Profile?.GetUrl(nameof(payload.Profile));
    Uri? website = payload.Website?.GetUrl(nameof(payload.Website));

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
    if (reference == null || locale != reference.Locale)
    {
      user.Locale = locale;
    }
    if (reference == null || gender != reference.Gender)
    {
      user.Gender = gender;
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
  }

  private static void SetCustomAttributes(UserAggregate user, UserAggregate? reference, ReplaceUserPayload payload)
  {
    HashSet<string> customAttributeKeys = payload.CustomAttributes.Select(x => x.Key.Trim()).ToHashSet();
    foreach (string key in user.CustomAttributes.Keys)
    {
      if (!customAttributeKeys.Contains(key) && (reference == null || reference.CustomAttributes.ContainsKey(key)))
      {
        user.RemoveCustomAttribute(key);
      }
    }
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      user.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
  }

  private async Task SetRolesAsync(UserAggregate user, UserAggregate? reference,
    ReplaceUserPayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    IEnumerable<RoleAggregate> roles = await _mediator.Send(new FindRolesQuery(payload.Roles, nameof(payload.Roles), realm), cancellationToken);

    HashSet<AggregateId> roleIds = roles.Select(r => r.Id).ToHashSet();
    foreach (AggregateId roleId in user.Roles)
    {
      if (!roleIds.Contains(roleId) && (reference == null || reference.Roles.Contains(roleId)))
      {
        user.RemoveRole(roleId);
      }
    }

    foreach (RoleAggregate role in roles)
    {
      user.AddRole(role);
    }
  }
}
