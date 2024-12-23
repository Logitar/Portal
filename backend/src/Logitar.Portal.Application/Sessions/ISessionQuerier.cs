using Logitar.Identity.Domain.Sessions;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions;

public interface ISessionQuerier
{
  Task<Session> ReadAsync(RealmModel? realm, SessionAggregate session, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(RealmModel? realm, SessionId id, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<Session>> SearchAsync(RealmModel? realm, SearchSessionsPayload payload, CancellationToken cancellationToken = default);
}
