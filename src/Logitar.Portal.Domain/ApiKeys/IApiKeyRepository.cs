using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;

namespace Logitar.Portal.Domain.ApiKeys;

public interface IApiKeyRepository
{
  Task<ApiKeyAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApiKeyAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken = default);
  Task<ApiKeyAggregate?> LoadAsync(AggregateId id, long? version, CancellationToken cancellationToken = default);
  Task<IEnumerable<ApiKeyAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken = default);
  Task<IEnumerable<ApiKeyAggregate>> LoadAsync(RoleAggregate role, CancellationToken cancellationToken = default);
  Task SaveAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ApiKeyAggregate> apiKeys, CancellationToken cancellationToken = default);
}
