using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class RenewCommandHandler : IRequestHandler<RenewCommand, Session>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public RenewCommandHandler(IApplicationContext applicationContext, IPasswordService passwordService,
    IRealmRepository realmRepository, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session> Handle(RenewCommand command, CancellationToken cancellationToken)
  {
    RenewPayload payload = command.Payload;

    RefreshToken refreshToken;
    try
    {
      refreshToken = RefreshToken.Decode(payload.RefreshToken);
    }
    catch (Exception innerException)
    {
      throw new InvalidRefreshTokenException(payload.RefreshToken, innerException);
    }

    SessionAggregate session = await _sessionRepository.LoadAsync(refreshToken.Id, cancellationToken)
      ?? throw new SessionNotFoundException(refreshToken.Id);

    RealmAggregate? realm = await _realmRepository.LoadAsync(session, cancellationToken);
    IPasswordSettings passwordSettings = realm?.PasswordSettings ?? _applicationContext.Configuration.PasswordSettings;

    Password newSecret = _passwordService.Generate(passwordSettings, SessionAggregate.SecretLength, out byte[] secretBytes);
    session.Renew(Convert.ToBase64String(refreshToken.Secret), newSecret, _applicationContext.ActorId);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    session.Update(_applicationContext.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    result.RefreshToken = new RefreshToken(session, secretBytes).Encode();

    return result;
  }
}
