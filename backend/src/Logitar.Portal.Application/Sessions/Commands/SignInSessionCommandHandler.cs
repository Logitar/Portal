using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Sessions.Validators;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class SignInSessionCommandHandler : IRequestHandler<SignInSessionCommand, Session>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserManager _userManager;

  public SignInSessionCommandHandler(IApplicationContext applicationContext, IPasswordManager passwordManager, ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository, IUserManager userManager)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userManager = userManager;
  }

  public async Task<Session> Handle(SignInSessionCommand command, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = command.Payload;
    new SignInSessionValidator().ValidateAndThrow(payload);

    SessionId? sessionId = SessionId.TryCreate(payload.Id);
    if (sessionId != null && await _sessionRepository.LoadAsync(sessionId, cancellationToken) != null)
    {
      throw new IdentifierAlreadyUsedException<SessionAggregate>(payload.Id!, nameof(payload.Id));
    }

    string? tenantId = _applicationContext.Realm?.Id;
    FoundUsers users = await _userManager.FindAsync(tenantId, payload.UniqueName, cancellationToken);
    UserAggregate user = users.FirstOrDefault() ?? throw new UserNotFoundException(payload.UniqueName, nameof(payload.UniqueName));
    ActorId actorId = new(user.Id.Value);

    Password? secret = null;
    string? secretString = null;
    if (payload.IsPersistent)
    {
      secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out secretString);
    }

    SessionAggregate session = user.SignIn(payload.Password, secret, actorId, sessionId);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    session.Update(actorId);

    await _userManager.SaveAsync(user, actorId, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    if (secretString != null)
    {
      result.RefreshToken = RefreshToken.Encode(session.Id, secretString);
    }
    return result;
  }
}
