using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Session>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public CreateSessionCommandHandler(IApplicationContext applicationContext, IPasswordService passwordService, IRealmRepository realmRepository,
    ISessionQuerier sessionQuerier, ISessionRepository sessionRepository, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<Session> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
  {
    CreateSessionPayload payload = command.Payload;

    UserAggregate user = await _userRepository.LoadAsync(payload.UserId, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(payload.UserId, nameof(payload.UserId));
    RealmAggregate? realm = user.TenantId == null ? null : await _realmRepository.LoadAsync(user, cancellationToken);

    IUserSettings userSettings = realm?.UserSettings ?? _applicationContext.Configuration.UserSettings;

    byte[]? secretBytes = null;
    Password? secret = payload.IsPersistent
      ? _passwordService.Generate(userSettings.PasswordSettings, SessionAggregate.SecretLength, out secretBytes)
      : null;
    SessionAggregate session = user.SignIn(userSettings, secret, _applicationContext.ActorId);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    session.Update(_applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    if (secretBytes != null)
    {
      result.RefreshToken = new RefreshToken(session, secretBytes).Encode();
    }

    return result;
  }
}
