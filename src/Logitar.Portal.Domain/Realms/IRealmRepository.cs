﻿using Logitar.EventSourcing;

namespace Logitar.Portal.Domain.Realms;

public interface IRealmRepository
{
  Task<RealmAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(AggregateId id, long? version, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
  Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<RealmAggregate> realms, CancellationToken cancellationToken = default);
}
