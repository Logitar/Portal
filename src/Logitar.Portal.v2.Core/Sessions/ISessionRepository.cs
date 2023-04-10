namespace Logitar.Portal.v2.Core.Sessions;

public interface ISessionRepository
{
  Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken = default);
}
