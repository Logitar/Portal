﻿using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Senders;

public interface ISenderRepository
{
  Task<SenderAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SenderAggregate?> LoadAsync(AggregateId id, long? version = null, CancellationToken cancellationToken = default);
  Task<IEnumerable<SenderAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken = default);
  Task<SenderAggregate?> LoadDefaultAsync(string? tenantId, CancellationToken cancellationToken = default);

  Task SaveAsync(SenderAggregate sender, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<SenderAggregate> senders, CancellationToken cancellationToken = default);
}