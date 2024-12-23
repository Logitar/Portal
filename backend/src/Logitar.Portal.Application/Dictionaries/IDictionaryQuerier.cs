using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries;

public interface IDictionaryQuerier
{
  Task<DictionaryModel> ReadAsync(Realm? realm, Dictionary dictionary, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReadAsync(Realm? realm, DictionaryId id, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReadAsync(Realm? realm, string locale, CancellationToken cancellationToken = default);
  Task<SearchResults<DictionaryModel>> SearchAsync(Realm? realm, SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
}
