namespace Logitar.Portal.Contracts.Realms;

public interface IRealmService
{
  Task<Realm> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken = default);
  Task<Realm?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(Guid? id = null, string? uniqueSlug = null, CancellationToken cancellationToken = default);
  Task<Realm?> ReplaceAsync(Guid id, ReplaceRealmPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken = default);
  Task<Realm?> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken = default);
}
