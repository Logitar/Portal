using Logitar.Identity.Domain.Roles;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Application.Roles;

public interface IRoleQuerier
{
  Task<Role> ReadAsync(RoleAggregate role, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(RoleId id, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(string id, CancellationToken cancellationToken = default);

  Task<Role?> ReadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken = default);
}
