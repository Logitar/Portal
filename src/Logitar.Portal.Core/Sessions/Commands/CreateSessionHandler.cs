using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal class CreateSessionHandler : IRequestHandler<CreateSession, Session>
{
  private readonly IRealmRepository _realmRepository;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public CreateSessionHandler(IRealmRepository realmRepository,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserRepository userRepository)
  {
    _realmRepository = realmRepository;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<Session> Handle(CreateSession request, CancellationToken cancellationToken)
  {
    CreateSessionInput input = request.Input;

    UserAggregate user = await _userRepository.LoadAsync(input.UserId, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(input.UserId, nameof(input.UserId));
    RealmAggregate? realm = await _realmRepository.LoadAsync(user, cancellationToken);

    SessionAggregate session = user.SignIn(realm, password: null, isPersistent: false,
      input.IpAddress, input.AdditionalInformation, input.CustomAttributes?.ToDictionary());

    await _userRepository.SaveAsync(user, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session output = await _sessionQuerier.GetAsync(session, cancellationToken);
    output.RefreshToken = session.RefreshToken?.ToString();

    return output;
  }
}
