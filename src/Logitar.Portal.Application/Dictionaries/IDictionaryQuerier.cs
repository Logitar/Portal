﻿using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries;

public interface IDictionaryQuerier
{
  Task<Dictionary> ReadAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(string? tenantId, string locale, CancellationToken cancellationToken = default);
  Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
}
