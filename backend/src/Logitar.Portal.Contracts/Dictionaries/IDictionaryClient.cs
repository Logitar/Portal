using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Dictionaries;

public interface IDictionaryClient
{
  Task<DictionaryModel> CreateAsync(CreateDictionaryPayload payload, IRequestContext? context = null);
  Task<DictionaryModel?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<DictionaryModel?> ReadAsync(Guid? id = null, string? locale = null, IRequestContext? context = null);
  Task<DictionaryModel?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<DictionaryModel>> SearchAsync(SearchDictionariesPayload payload, IRequestContext? context = null);
  Task<DictionaryModel?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, IRequestContext? context = null);
}
