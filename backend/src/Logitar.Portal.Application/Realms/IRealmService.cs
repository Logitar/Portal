using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Realms;

public interface IRealmService
{
  Task<RealmModel> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken = default);
  Task<RealmModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(Guid? id = null, string? uniqueSlug = null, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReplaceAsync(Guid id, ReplaceRealmPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<RealmModel>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken = default);
  Task<RealmModel?> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken = default);
}
