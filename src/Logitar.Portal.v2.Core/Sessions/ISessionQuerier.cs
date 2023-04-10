using Logitar.Portal.v2.Contracts.Sessions;

namespace Logitar.Portal.v2.Core.Sessions;

public interface ISessionQuerier
{
  Task<Session> GetAsync(SessionAggregate session, CancellationToken cancellationToken = default);
}
