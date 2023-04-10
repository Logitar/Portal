using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Commands;

internal class DeleteSessionsHandler : IRequestHandler<DeleteSessions>
{
  private readonly ICurrentActor _currentActor;
  private readonly ISessionRepository _sessionRepository;

  public DeleteSessionsHandler(ICurrentActor currentActor, ISessionRepository sessionRepository)
  {
    _currentActor = currentActor;
    _sessionRepository = sessionRepository;
  }

  public async Task Handle(DeleteSessions request, CancellationToken cancellationToken)
  {
    if (request.Realm != null)
    {
      IEnumerable<SessionAggregate> sessions = await _sessionRepository.LoadAsync(request.Realm, cancellationToken);
      foreach (SessionAggregate session in sessions)
      {
        session.Delete(_currentActor.Id);
      }

      await _sessionRepository.SaveAsync(sessions, cancellationToken);
    }

    if (request.User != null)
    {
      IEnumerable<SessionAggregate> sessions = await _sessionRepository.LoadAsync(request.User, cancellationToken);
      foreach (SessionAggregate session in sessions)
      {
        session.Delete(_currentActor.Id);
      }

      await _sessionRepository.SaveAsync(sessions, cancellationToken);
    }
  }
}
