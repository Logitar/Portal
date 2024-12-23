﻿using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Dictionaries;

public interface IDictionaryRepository
{
  Task<DictionaryAggregate?> LoadAsync(DictionaryId id, long? version = null, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<DictionaryAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<DictionaryAggregate>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);
  Task<DictionaryAggregate?> LoadAsync(TenantId? tenantId, Locale locale, CancellationToken cancellationToken = default);

  Task SaveAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<DictionaryAggregate> dictionaries, CancellationToken cancellationToken = default);
}
