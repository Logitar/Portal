using Logitar.Identity.Domain.Sessions;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions;

public interface ISessionQuerier
{
  Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
