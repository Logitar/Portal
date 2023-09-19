using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Sessions;

namespace Logitar.Portal.Application.Sessions;

public interface ISessionQuerier
{
  Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken = default);
}
