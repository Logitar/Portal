using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Dictionaries;

public interface IDictionaryRepository
{
  Task<DictionaryAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<DictionaryAggregate?> LoadAsync(AggregateId id, long? version, CancellationToken cancellationToken = default);
  Task<DictionaryAggregate?> LoadAsync(string? tenantId, Locale locale, CancellationToken cancellationToken = default);
  Task<IEnumerable<DictionaryAggregate>> LoadAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task SaveAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<DictionaryAggregate> dictionaries, CancellationToken cancellationToken = default);
}
