using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Sessions.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class RenewSessionCommandHandler : IRequestHandler<RenewSessionCommand, SessionModel>
{
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public RenewSessionCommandHandler(IPasswordManager passwordManager,
    ISessionQuerier sessionQuerier, ISessionRepository sessionRepository, IUserRepository userRepository)
  {
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<SessionModel> Handle(RenewSessionCommand command, CancellationToken cancellationToken)
  {
    RenewSessionPayload payload = command.Payload;
    new RenewSessionValidator().ValidateAndThrow(payload);

    RefreshToken refreshToken;
    try
    {
      refreshToken = RefreshToken.Decode(payload.RefreshToken);
    }
    catch (Exception innerException)
    {
      throw new InvalidRefreshTokenException(payload.RefreshToken, nameof(payload.RefreshToken), innerException);
    }

    SessionAggregate session = await _sessionRepository.LoadAsync(refreshToken.Id, cancellationToken)
      ?? throw new SessionNotFoundException(refreshToken.Id, nameof(payload.RefreshToken));
    UserAggregate user = await _userRepository.LoadAsync(session, cancellationToken);
    if (user.TenantId != command.TenantId)
    {
      throw new SessionNotFoundException(session.Id, nameof(payload.RefreshToken));
    }

    ActorId actorId = command.Actor.Type == ActorType.System
      ? new(session.UserId.Value)
      : command.ActorId;

    Password newSecret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    session.Renew(refreshToken.Secret, newSecret, actorId);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    session.Update(actorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    SessionModel result = await _sessionQuerier.ReadAsync(command.Realm, session, cancellationToken);
    result.RefreshToken = RefreshToken.Encode(session.Id, secretString);
    return result;
  }
}
