using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Sessions.Validators;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Session>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserManager _userManager;

  public CreateSessionCommandHandler(IApplicationContext applicationContext, IPasswordManager passwordManager,
    ISessionQuerier sessionQuerier, ISessionRepository sessionRepository, IUserManager userManager)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userManager = userManager;
  }

  public async Task<Session> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
  {
    CreateSessionPayload payload = command.Payload;
    new CreateSessionValidator().ValidateAndThrow(payload);

    TenantId? tenantId = _applicationContext.TenantId;
    FoundUsers users = await _userManager.FindAsync(tenantId?.Value, payload.User, cancellationToken);
    UserAggregate user = users.SingleOrDefault() ?? throw new UserNotFoundException(tenantId, payload.User, nameof(payload.User));

    Password? secret = null;
    string? secretString = null;
    if (payload.IsPersistent)
    {
      secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out secretString);
    }

    ActorId actorId = _applicationContext.ActorId;
    SessionAggregate session = user.SignIn(secret, actorId);
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
