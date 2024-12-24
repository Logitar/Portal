using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries;

public interface IDictionaryQuerier
{
  Task<DictionaryModel> ReadAsync(RealmModel? realm, Dictionary dictionary, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReadAsync(RealmModel? realm, DictionaryId id, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReadAsync(RealmModel? realm, string locale, CancellationToken cancellationToken = default);
  Task<SearchResults<DictionaryModel>> SearchAsync(RealmModel? realm, SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
}
