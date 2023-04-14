using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal class DeleteSessionsHandler : IRequestHandler<DeleteSessions>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionRepository _sessionRepository;

  public DeleteSessionsHandler(IApplicationContext applicationContext, ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _sessionRepository = sessionRepository;
  }

  public async Task Handle(DeleteSessions request, CancellationToken cancellationToken)
  {
    if (request.Realm != null)
    {
      IEnumerable<SessionAggregate> sessions = await _sessionRepository.LoadAsync(request.Realm, cancellationToken);
      foreach (SessionAggregate session in sessions)
      {
        session.Delete(_applicationContext.ActorId);
      }

      await _sessionRepository.SaveAsync(sessions, cancellationToken);
    }

    if (request.User != null)
    {
      IEnumerable<SessionAggregate> sessions = await _sessionRepository.LoadAsync(request.User, cancellationToken);
      foreach (SessionAggregate session in sessions)
      {
        session.Delete(_applicationContext.ActorId);
      }

      await _sessionRepository.SaveAsync(sessions, cancellationToken);
    }
  }
}
