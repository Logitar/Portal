namespace Logitar.Portal.Contracts.Realms;

public interface IRealmService
{
  Task<Realm> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken = default);
  Task<Realm?> DeleteAsync(string id, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(string? id = null, string? uniqueSlug = null, CancellationToken cancellationToken = default);
  Task<Realm?> ReplaceAsync(string id, ReplaceRealmPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken = default);
  Task<Realm?> UpdateAsync(string id, UpdateRealmPayload payload, CancellationToken cancellationToken = default);
}
