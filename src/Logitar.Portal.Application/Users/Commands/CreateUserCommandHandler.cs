using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public CreateUserCommandHandler(IApplicationContext applicationContext,
    IPasswordService passwordService, IRealmRepository realmRepository,
    IUserManager userManager, IUserQuerier userQuerier)
  {
    _applicationContext = applicationContext;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
    _userManager = userManager;
    _userQuerier = userQuerier;
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
    IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings!; // TODO(fpion): use configuration
    string? tenantId = realm?.Id.Value;

    UserAggregate user = new(uniqueNameSettings, payload.UniqueName, tenantId, _applicationContext.ActorId);
    if (payload.Password != null)
    {
      user.SetPassword(_passwordService.Create(payload.Password));
    }
    if (payload.IsDisabled)
    {
      user.Disable();
    }

    if (payload.Address != null)
    {
      user.Address = payload.Address.ToPostalAddress(payload.Address.IsVerified ?? false);
    }
    if (payload.Email != null)
    {
      user.Email = payload.Email.ToEmailAddress(payload.Email.IsVerified ?? false);
    }
    if (payload.Phone != null)
    {
      user.Phone = payload.Phone.ToPhoneNumber(payload.Phone.IsVerified ?? false);
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

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      user.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    user.Update(_applicationContext.ActorId);

    await _userManager.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
