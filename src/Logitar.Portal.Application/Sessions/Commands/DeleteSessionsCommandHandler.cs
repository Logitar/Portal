using Logitar.Portal.Domain.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal class DeleteSessionsCommandHandler : INotificationHandler<DeleteSessionsCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionRepository _sessionRepository;

  public DeleteSessionsCommandHandler(IApplicationContext applicationContext, ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _sessionRepository = sessionRepository;
  }

  public async Task Handle(DeleteSessionsCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<SessionAggregate> sessions = await LoadAsync(command, cancellationToken);
    foreach (SessionAggregate session in sessions)
    {
      session.Delete(_applicationContext.ActorId);
    }

    await _sessionRepository.SaveAsync(sessions, cancellationToken);
  }

  private async Task<IEnumerable<SessionAggregate>> LoadAsync(DeleteSessionsCommand command, CancellationToken cancellationToken)
  {
    if (command.Realm != null)
    {
      return await _sessionRepository.LoadAsync(command.Realm, cancellationToken);
    }

    if (command.User != null)
    {
      return await _sessionRepository.LoadAsync(command.User, cancellationToken);
    }

    return Enumerable.Empty<SessionAggregate>();
  }
}
