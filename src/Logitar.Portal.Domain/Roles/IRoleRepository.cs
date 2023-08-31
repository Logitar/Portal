using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Roles;

public interface IRoleRepository
{
  Task<RoleAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RoleAggregate?> LoadAsync(AggregateId id, long? version = null, CancellationToken cancellationToken = default);
  Task<RoleAggregate?> LoadAsync(string? tenantId, string uniqueName, CancellationToken cancellationToken = default);
  Task<IEnumerable<RoleAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken = default);
  Task SaveAsync(RoleAggregate role, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<RoleAggregate> roles, CancellationToken cancellationToken = default);
}
