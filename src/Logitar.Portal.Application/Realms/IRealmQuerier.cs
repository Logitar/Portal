using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application.Realms;

public interface IRealmQuerier
{
  Task<Realm?> ReadAsync(AggregateId id, CancellationToken cancellationToken);
}
