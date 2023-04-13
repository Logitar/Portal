using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Core.Dictionaries;

public interface IDictionaryQuerier
{
  Task<Dictionary> GetAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken = default);
  Task<Dictionary?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PagedList<Dictionary>> GetAsync(string? locale = null, string? realm = null, DictionarySort? sort = null,
    bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
}
