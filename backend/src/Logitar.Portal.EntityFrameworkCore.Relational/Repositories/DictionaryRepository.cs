﻿using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class DictionaryRepository : Repository, IDictionaryRepository
{
  private readonly DbSet<DictionaryEntity> _dictionaries;

  public DictionaryRepository(PortalContext context, IEventStore eventStore) : base(eventStore)
  {
    _dictionaries = context.Dictionaries;
  }

  public async Task<Dictionary?> LoadAsync(DictionaryId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Dictionary?> LoadAsync(DictionaryId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Dictionary>(id.StreamId, version, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Dictionary>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Dictionary>(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Dictionary>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    Guid? tenantIdValue = tenantId?.ToGuid();

    IEnumerable<StreamId> streamIds = (await _dictionaries.AsNoTracking()
      .Where(x => x.TenantId == tenantIdValue)
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken)).Select(value => new StreamId(value));

    return await LoadAsync<Dictionary>(streamIds, cancellationToken);
  }

  public async Task<Dictionary?> LoadAsync(TenantId? tenantId, Locale locale, CancellationToken cancellationToken)
  {
    Guid? tenantIdValue = tenantId?.ToGuid();
    string localeNormalized = Helper.Normalize(locale.Code);

    string? streamId = await _dictionaries.AsNoTracking()
      .Where(x => x.TenantId == tenantIdValue && x.LocaleNormalized == localeNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (streamId == null)
    {
      return null;
    }

    return await LoadAsync<Dictionary>(new StreamId(streamId), cancellationToken);
  }

  public async Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken)
  {
    await base.SaveAsync(dictionary, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Dictionary> dictionaries, CancellationToken cancellationToken)
  {
    await base.SaveAsync(dictionaries, cancellationToken);
  }
}
