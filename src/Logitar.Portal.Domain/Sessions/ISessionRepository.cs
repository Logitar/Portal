using Logitar.EventSourcing;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Sessions;

public interface ISessionRepository
{
  Task<SessionAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SessionAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken = default);
  Task<IEnumerable<SessionAggregate>> LoadAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task<IEnumerable<SessionAggregate>> LoadAsync(UserAggregate user, bool? isActive, CancellationToken cancellationToken = default);
  Task SaveAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken = default);
}
