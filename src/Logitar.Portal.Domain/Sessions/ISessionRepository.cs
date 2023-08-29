using Logitar.EventSourcing;

namespace Logitar.Portal.Domain.Sessions;

public interface ISessionRepository
{
  Task<SessionAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SessionAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken = default);
  Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken = default);
}
