using Logitar.Identity.Domain.Roles;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Roles;

public interface IRoleQuerier
{
  Task<Role> ReadAsync(RoleAggregate role, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(RoleId id, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken = default);
}
