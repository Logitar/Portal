using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Roles;

public interface IRoleService
{
  Task<RoleModel> CreateAsync(CreateRolePayload payload, CancellationToken cancellationToken = default);
  Task<RoleModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RoleModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<RoleModel?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<RoleModel>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken = default);
  Task<RoleModel?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken = default);
}
