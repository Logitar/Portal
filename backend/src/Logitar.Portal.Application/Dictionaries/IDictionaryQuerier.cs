using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain;
using System.Globalization;

namespace Logitar.Portal.Application.Dictionaries
{
  public interface IDictionaryQuerier
  {
    Task<DictionaryModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
    Task<DictionaryModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<DictionaryModel>> GetPagedAsync(CultureInfo? locale = null, string? realm = null,
      DictionarySort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
