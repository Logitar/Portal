using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Sessions.Validators;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal record CreateSessionCommand(CreateSessionPayload Payload) : Activity, IRequest<SessionModel>;

internal class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, SessionModel>
{
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserManager _userManager;

  public CreateSessionCommandHandler(
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

  public async Task<SessionModel> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = command.UserSettings;

    CreateSessionPayload payload = command.Payload;
    new CreateSessionValidator().ValidateAndThrow(payload);

    FindUserQuery query = new(command.TenantId, payload.User, command.UserSettings, nameof(payload.User), IncludeId: true);
    User user = await _mediator.Send(query, cancellationToken);

    Password? secret = null;
    string? secretString = null;
    if (payload.IsPersistent)
    {
      secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out secretString);
    }

    ActorId actorId = command.ActorId;
    Session session = user.SignIn(secret, actorId);
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
