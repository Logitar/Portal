using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public CreateUserCommandHandler(IApplicationContext applicationContext, IPasswordService passwordService,
    IRealmRepository realmRepository, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
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
    IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;

    if (await _userRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException<UserAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
    }

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

    if (payload.Address != null)
    {
      user.Address = new PostalAddress(payload.Address.Street, payload.Address.Locality,
        payload.Address.Country, payload.Address.Region, payload.Address.PostalCode, payload.Address.IsVerified);
    }
    if (payload.Email != null)
    {
      if (realm?.RequireUniqueEmail == true)
      {
        IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(tenantId, payload.Email, cancellationToken);
        if (users.Any(u => !u.Equals(user)))
        {
          throw new EmailAddressAlreadyUsedException(tenantId, payload.Email, nameof(payload.Email));
        }
      }

      user.Email = new EmailAddress(payload.Email.Address, payload.Email.IsVerified);
    }
    if (payload.Phone != null)
    {
      user.Phone = new PhoneNumber(payload.Phone.Number, payload.Phone.CountryCode, payload.Phone.Extension, payload.Phone.IsVerified);
    }

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

    // TODO(fpion): Roles

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      user.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    user.Update(_applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
