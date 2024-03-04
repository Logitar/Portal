using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries;

public interface IDictionaryQuerier
{
  Task<Dictionary> ReadAsync(Realm? realm, DictionaryAggregate dictionary, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(Realm? realm, DictionaryId id, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(Realm? realm, string locale, CancellationToken cancellationToken = default);
  Task<SearchResults<Dictionary>> SearchAsync(Realm? realm, SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
}
