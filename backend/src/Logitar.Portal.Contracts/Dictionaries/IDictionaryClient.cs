using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Dictionaries;

public interface IDictionaryClient
{
  Task<Dictionary> CreateAsync(CreateDictionaryPayload payload, IRequestContext? context = null);
  Task<Dictionary?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<Dictionary?> ReadAsync(Guid? id = null, string? locale = null, IRequestContext? context = null);
  Task<Dictionary?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, IRequestContext? context = null);
  Task<Dictionary?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, IRequestContext? context = null);
}
