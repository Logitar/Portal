using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Session>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly ISessionManager _sessionManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly IUserManager _userManager;
  private readonly IUserRepository _userRepository;

  public CreateSessionCommandHandler(IApplicationContext applicationContext,
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

  public async Task<Session> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
  {
    CreateSessionPayload payload = command.Payload;

    AggregateId userId = payload.UserId.GetAggregateId(nameof(payload.UserId));
    UserAggregate user = await _userRepository.LoadAsync(userId, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(payload.UserId, nameof(payload.UserId));

    RealmAggregate? realm = null;
    if (user.TenantId != null)
    {
      realm = await _realmRepository.FindAsync(user.TenantId, cancellationToken)
        ?? throw new InvalidOperationException($"The realm '{user.TenantId}' could not be found from user '{user}'.");
    }
    IUserSettings userSettings = realm?.UserSettings!; // TODO(fpion): use configuration

    byte[]? secretBytes = null;
    Password? secret = payload.IsPersistent ? _passwordService.Generate(Constants.SecretLength, out secretBytes) : null;
    SessionAggregate session = user.SignIn(userSettings, secret, _applicationContext.ActorId);

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
