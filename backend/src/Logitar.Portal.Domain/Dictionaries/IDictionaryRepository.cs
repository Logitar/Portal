using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Dictionaries;

public interface IDictionaryRepository
{
  Task<Dictionary?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Dictionary?> LoadAsync(DictionaryId id, long? version = null, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Dictionary>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Dictionary>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);
  Task<Dictionary?> LoadAsync(TenantId? tenantId, LocaleUnit locale, CancellationToken cancellationToken = default);

  Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Dictionary> dictionaries, CancellationToken cancellationToken = default);
}
