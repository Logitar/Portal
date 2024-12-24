using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Dictionaries;

public interface IDictionaryRepository
{
  Task<Dictionary?> LoadAsync(DictionaryId id, CancellationToken cancellationToken = default);
  Task<Dictionary?> LoadAsync(DictionaryId id, long? version, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Dictionary>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Dictionary>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);
  Task<Dictionary?> LoadAsync(TenantId? tenantId, Locale locale, CancellationToken cancellationToken = default);

  Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Dictionary> dictionaries, CancellationToken cancellationToken = default);
}
