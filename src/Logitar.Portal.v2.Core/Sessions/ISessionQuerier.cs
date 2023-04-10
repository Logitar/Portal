using Logitar.Portal.v2.Contracts.Sessions;

namespace Logitar.Portal.v2.Core.Sessions;

public interface ISessionQuerier
{
  Task<Session> GetAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<IEnumerable<Session>> GetAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken = default);
}
