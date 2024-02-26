using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Dictionaries;

public interface IDictionaryRepository
{
  Task<DictionaryAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<DictionaryAggregate?> LoadAsync(DictionaryId id, long? version = null, CancellationToken cancellationToken = default);
  Task<IEnumerable<DictionaryAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<DictionaryAggregate>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);
  Task<DictionaryAggregate?> LoadAsync(TenantId? tenantId, LocaleUnit locale, CancellationToken cancellationToken = default);

  Task SaveAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<DictionaryAggregate> dictionaries, CancellationToken cancellationToken = default);
}
