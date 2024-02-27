using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Realms;

public interface IRealmClient
{
  Task<Realm> CreateAsync(CreateRealmPayload payload, IRequestContext? context = null);
  Task<Realm?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<Realm?> ReadAsync(Guid? id = null, string? uniqueSlug = null, IRequestContext? context = null);
  Task<Realm?> ReplaceAsync(Guid id, ReplaceRealmPayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, IRequestContext? context = null);
  Task<Realm?> UpdateAsync(Guid id, UpdateRealmPayload payload, IRequestContext? context = null);
}
