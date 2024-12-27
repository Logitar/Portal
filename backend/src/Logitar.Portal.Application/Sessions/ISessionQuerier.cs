using Logitar.Identity.Core.Sessions;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions;

public interface ISessionQuerier
{
  Task<SessionModel> ReadAsync(RealmModel? realm, Session session, CancellationToken cancellationToken = default);
  Task<SessionModel?> ReadAsync(RealmModel? realm, SessionId id, CancellationToken cancellationToken = default);
  Task<SessionModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<SessionModel>> SearchAsync(RealmModel? realm, SearchSessionsPayload payload, CancellationToken cancellationToken = default);
}
