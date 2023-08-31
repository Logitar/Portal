using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.Roles;

namespace Logitar.Portal.Application.Roles;
public interface IRoleQuerier
{
  Task<Role> ReadAsync(RoleAggregate role, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(string? tenantId, string uniqueName, CancellationToken cancellationToken = default);
  Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken = default);
}
