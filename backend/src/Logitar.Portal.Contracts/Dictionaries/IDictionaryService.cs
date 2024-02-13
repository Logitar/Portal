using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Dictionaries;

public interface IDictionaryService
{
  Task<Dictionary> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken = default);
  Task<Dictionary?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(Guid? id = null, string? locale = null, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReplaceAsync(Guid id, ReplaceDictionaryPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
  Task<Dictionary?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken = default);
}
