using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Roles;

public interface IRoleService
{
  Task<Role> CreateAsync(CreateRolePayload payload, CancellationToken cancellationToken = default);
  Task<Role?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<Role?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken = default);
  Task<Role?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken = default);
}
