using Logitar.Portal.Core;
using Logitar.Portal.Core.Dictionaries;
using Logitar.Portal.Core.Dictionaries.Models;
using Logitar.Portal.Core.Dictionaries.Payloads;

namespace Logitar.Portal.Application.Dictionaries
{
  public interface IDictionaryService
  {
    Task<DictionaryModel> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken = default);
    Task<DictionaryModel> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DictionaryModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListModel<DictionaryModel>> GetAsync(string? locale = null, string? realm = null,
      DictionarySort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<DictionaryModel> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken = default);
  }
}
