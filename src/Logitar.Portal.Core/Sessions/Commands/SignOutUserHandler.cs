using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal class SignOutUserHandler : IRequestHandler<SignOutUser, IEnumerable<Session>>
{
  private readonly ICurrentActor _currentActor;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutUserHandler(ICurrentActor currentActor,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<IEnumerable<Session>> Handle(SignOutUser request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    IEnumerable<SessionAggregate> sessions = await _sessionRepository.LoadActiveAsync(user, cancellationToken);
    foreach (SessionAggregate session in sessions)
    {
      session.SignOut(_currentActor.Id);
    }

    await _sessionRepository.SaveAsync(sessions, cancellationToken);

    return await _sessionQuerier.GetAsync(sessions, cancellationToken);
  }
}
