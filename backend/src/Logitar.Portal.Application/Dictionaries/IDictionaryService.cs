using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries
{
  public interface IDictionaryService
  {
    Task<DictionaryModel> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<DictionaryModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<DictionaryModel>> GetAsync(string? locale = null, string? realm = null,
      DictionarySort? sort = null, bool isDescending = false, int? index = null, int? count = null, CancellationToken cancellationToken = default);
    Task<DictionaryModel> UpdateAsync(string id, UpdateDictionaryPayload payload, CancellationToken cancellationToken = default);
  }
}
