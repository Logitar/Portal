using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Sessions;

namespace Logitar.Portal.v2.Core.Sessions;

public interface ISessionQuerier
{
  Task<Session> GetAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<Session?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<Session>> GetAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken = default);
  Task<PagedList<Session>> GetAsync(bool? isActive = null, bool? isPersistent = null, string? realm = null, Guid? userId = null,
    SessionSort? sort = null, bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
}
