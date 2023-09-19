using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public AuthenticateUserCommandHandler(IApplicationContext applicationContext, IRealmRepository realmRepository, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;

    RealmAggregate? realm = null;
    if (payload.Realm != null)
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
        ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }
    string? tenantId = realm?.Id.Value;
    IUserSettings userSettings = realm?.UserSettings ?? _applicationContext.Configuration.UserSettings;

    UserAggregate? user = await _userRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken);
    if (user == null && realm?.RequireUniqueEmail == true)
    {
      Email email = new(payload.UniqueName);
      IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(tenantId, email, cancellationToken);
      if (users.Count() == 1)
      {
        user = users.Single();
      }
    }
    if (user == null)
    {
      throw new UserNotFoundException(realm, payload.UniqueName);
    }

    _ = user.SignIn(userSettings, payload.Password, actorId: _applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
