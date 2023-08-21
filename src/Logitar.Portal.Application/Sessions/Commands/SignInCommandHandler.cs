using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class SignInCommandHandler : IRequestHandler<SignInCommand, Session>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly ISessionManager _sessionManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly IUserManager _userManager;
  private readonly IUserRepository _userRepository;

  public SignInCommandHandler(IApplicationContext applicationContext,
    IPasswordService passwordService, IRealmRepository realmRepository,
    ISessionManager sessionManager, ISessionQuerier sessionQuerier,
    IUserManager userManager, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
    _sessionManager = sessionManager;
    _sessionQuerier = sessionQuerier;
    _userManager = userManager;
    _userRepository = userRepository;
  }

  public async Task<Session> Handle(SignInCommand command, CancellationToken cancellationToken)
  {
    SignInPayload payload = command.Payload;

    RealmAggregate? realm = null;
    if (payload.Realm != null)
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
        ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }
    IUserSettings userSettings = realm?.UserSettings ?? _applicationContext.Configuration.UserSettings;
    string? tenantId = realm?.Id.Value;

    UserAggregate user = await _userRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken)
      ?? throw new UserNotFoundException(realm, payload.UniqueName);

    byte[]? secretBytes = null;
    Password? secret = payload.IsPersistent ? _passwordService.Generate(Constants.SecretLength, out secretBytes) : null;
    SessionAggregate session = user.SignIn(userSettings, payload.Password, secret, _applicationContext.ActorId);

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    await _userManager.SaveAsync(user, cancellationToken);
    await _sessionManager.SaveAsync(session, cancellationToken);

    Session result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    if (secretBytes != null)
    {
      result.RefreshToken = new RefreshToken(session, secretBytes).Encode();
    }

    return result;
  }
}
