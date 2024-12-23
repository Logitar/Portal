using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries;

public interface IDictionaryQuerier
{
  Task<Dictionary> ReadAsync(RealmModel? realm, DictionaryAggregate dictionary, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(RealmModel? realm, DictionaryId id, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(RealmModel? realm, string locale, CancellationToken cancellationToken = default);
  Task<SearchResults<Dictionary>> SearchAsync(RealmModel? realm, SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
}
