namespace Logitar.Portal.v2.Contracts.Dictionaries;

public interface IDictionaryService
{
  Task<Dictionary> CreateAsync(CreateDictionaryInput input, CancellationToken cancellationToken = default);
  Task<Dictionary> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Dictionary?> GetAsync(Guid? id = null, CancellationToken cancellationToken = default);
  Task<PagedList<Dictionary>> GetAsync(string? locale = null, string? realm = null, DictionarySort? sort = null,
    bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
  Task<Dictionary> UpdateAsync(Guid id, UpdateDictionaryInput input, CancellationToken cancellationToken = default);
}
