using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Dictionaries;

public interface IDictionaryService
{
  Task<DictionaryModel> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReadAsync(Guid? id = null, string? locale = null, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<DictionaryModel>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken = default);
}
