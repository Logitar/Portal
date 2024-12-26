using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Realms;

public interface IRealmClient
{
  Task<RealmModel> CreateAsync(CreateRealmPayload payload, IRequestContext? context = null);
  Task<RealmModel?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<RealmModel?> ReadAsync(Guid? id = null, string? uniqueSlug = null, IRequestContext? context = null);
  Task<RealmModel?> ReplaceAsync(Guid id, ReplaceRealmPayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<RealmModel>> SearchAsync(SearchRealmsPayload payload, IRequestContext? context = null);
  Task<RealmModel?> UpdateAsync(Guid id, UpdateRealmPayload payload, IRequestContext? context = null);
}
