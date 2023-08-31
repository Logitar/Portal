using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly IRoleRepository _roleRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public CreateUserCommandHandler(IApplicationContext applicationContext, IPasswordService passwordService,
    IRealmRepository realmRepository, IRoleRepository roleRepository, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
    _roleRepository = roleRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken)
  {
    CreateUserPayload payload = command.Payload;

    RealmAggregate? realm = null;
    if (payload.Realm != null)
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
        ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }
    string? tenantId = realm?.Id.Value;

    if (await _userRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException<UserAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
    }

    UserAggregate user = CreateWithAuthenticationInformation(payload, realm);

    await SetContactInformationAsync(user, payload, realm, cancellationToken);
    SetPersonalInformation(user, payload);
    SetCustomAttributes(user, payload);
    await SetRolesAsync(user, payload, realm, cancellationToken);

    user.Update(_applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }

  private UserAggregate CreateWithAuthenticationInformation(CreateUserPayload payload, RealmAggregate? realm)
  {
    string? tenantId = realm?.Id.Value;
    IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;
    UserAggregate user = new(uniqueNameSettings, payload.UniqueName, tenantId, _applicationContext.ActorId);

    if (payload.Password != null)
    {
      IPasswordSettings passwordSettings = realm?.PasswordSettings ?? _applicationContext.Configuration.PasswordSettings;
      user.SetPassword(_passwordService.Create(passwordSettings, payload.Password));
    }

    if (payload.IsDisabled)
    {
      user.Disable(_applicationContext.ActorId);
    }

    return user;
  }

  private async Task SetContactInformationAsync(UserAggregate user, CreateUserPayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.Id.Value;
    PostalAddress? address = payload.Address?.ToPostalAddress();
    EmailAddress? email = payload.Email?.ToEmailAddress();
    PhoneNumber? phone = payload.Phone?.ToPhoneNumber();

    if (address != null)
    {
      user.Address = address;
    }
    if (email != null)
    {
      if (realm?.RequireUniqueEmail == true)
      {
        IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(tenantId, email, cancellationToken);
        if (users.Any(u => !u.Equals(user)))
        {
          throw new EmailAddressAlreadyUsedException(tenantId, email, nameof(payload.Email));
        }
      }

      user.Email = email;
    }
    if (phone != null)
    {
      user.Phone = phone;
    }
  }

  private static void SetPersonalInformation(UserAggregate user, CreateUserPayload payload)
  {
    user.FirstName = payload.FirstName;
    user.MiddleName = payload.MiddleName;
    user.LastName = payload.LastName;
    user.Nickname = payload.Nickname;

    user.Birthdate = payload.Birthdate;
    user.Gender = payload.Gender?.GetGender(nameof(payload.Gender));
    user.Locale = payload.Locale?.GetLocale(nameof(payload.Locale));
    user.TimeZone = payload.TimeZone?.GetTimeZone(nameof(payload.TimeZone));

    user.Picture = payload.Picture?.GetUrl(nameof(payload.Picture));
    user.Profile = payload.Profile?.GetUrl(nameof(payload.Profile));
    user.Website = payload.Website?.GetUrl(nameof(payload.Website));
  }

  private static void SetCustomAttributes(UserAggregate user, CreateUserPayload payload)
  {
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      user.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
  }

  private async Task SetRolesAsync(UserAggregate user, CreateUserPayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    IEnumerable<RoleAggregate> roles = await FindRolesAsync(payload, realm, cancellationToken);
    foreach (RoleAggregate role in roles)
    {
      user.AddRole(role);
    }
  }
  private async Task<IEnumerable<RoleAggregate>> FindRolesAsync(CreateUserPayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    int roleCount = payload.Roles.Count();
    if (roleCount == 0)
    {
      return Enumerable.Empty<RoleAggregate>();
    }

    IEnumerable<RoleAggregate> roles = await _roleRepository.LoadAsync(realm, cancellationToken);
    Dictionary<Guid, RoleAggregate> rolesById = new(capacity: roles.Count());
    Dictionary<string, RoleAggregate> rolesByUniqueName = new(capacity: rolesById.Count);
    foreach (RoleAggregate role in roles)
    {
      rolesById[role.Id.ToGuid()] = role;
      rolesByUniqueName[role.UniqueName.ToUpper()] = role;
    }

    List<RoleAggregate> foundRoles = new(capacity: roleCount);
    List<string> missingRoles = new(capacity: roleCount);

    foreach (string idOrUniqueName in payload.Roles)
    {
      if (!string.IsNullOrWhiteSpace(idOrUniqueName))
      {
        if ((Guid.TryParse(idOrUniqueName.Trim(), out Guid id) && rolesById.TryGetValue(id, out RoleAggregate? role))
          || rolesByUniqueName.TryGetValue(idOrUniqueName.Trim().ToUpper(), out role))
        {
          foundRoles.Add(role);
        }
        else
        {
          missingRoles.Add(idOrUniqueName);
        }
      }
    }

    if (missingRoles.Any())
    {
      throw new RolesNotFoundException(missingRoles, nameof(payload.Roles));
    }

    return foundRoles.AsReadOnly();
  }
}
