namespace Logitar.Portal.Domain.Sessions;

public interface ISessionRepository
{
  Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken = default);
}
