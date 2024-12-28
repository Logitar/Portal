using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Sessions.Validators;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

public record SignInSessionCommand(SignInSessionPayload Payload) : Activity, IRequest<SessionModel>
{
  public override IActivity Anonymize()
  {
    SignInSessionCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}

internal class SignInSessionCommandHandler : IRequestHandler<SignInSessionCommand, SessionModel>
{
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserManager _userManager;

  public SignInSessionCommandHandler(
    IMediator mediator,
    IPasswordManager passwordManager,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserManager userManager)
  {
    _mediator = mediator;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userManager = userManager;
  }

  public async Task<SessionModel> Handle(SignInSessionCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = command.UserSettings;

    SignInSessionPayload payload = command.Payload;
    new SignInSessionValidator().ValidateAndThrow(payload);

    SessionId sessionId = SessionId.NewId(command.TenantId);
    Session? session;
    if (payload.Id.HasValue)
    {
      sessionId = new(command.TenantId, new EntityId(payload.Id.Value));
      session = await _sessionRepository.LoadAsync(sessionId, cancellationToken);
      if (session != null)
      {
        throw new IdAlreadyUsedException(payload.Id.Value, nameof(payload.Id));
      }
    }

    FindUserQuery query = new(command.TenantId, payload.UniqueName, userSettings, nameof(payload.UniqueName));
    User user = await _mediator.Send(query, cancellationToken);
    ActorId actorId = new(user.Id.Value);

    Password? secret = null;
    string? secretString = null;
    if (payload.IsPersistent)
    {
      secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out secretString);
    }

    session = user.SignIn(payload.Password, secret, actorId, sessionId.EntityId);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      session.SetCustomAttribute(key, customAttribute.Value);
    }
    session.Update(actorId);

    await _userManager.SaveAsync(user, userSettings, actorId, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    SessionModel result = await _sessionQuerier.ReadAsync(command.Realm, session, cancellationToken);
    if (secretString != null)
    {
      result.RefreshToken = RefreshToken.Encode(session.Id, secretString);
    }
    return result;
  }
}
