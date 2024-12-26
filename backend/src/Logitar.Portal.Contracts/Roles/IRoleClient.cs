using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Roles;

public interface IRoleClient
{
  Task<RoleModel> CreateAsync(CreateRolePayload payload, IRequestContext? context = null);
  Task<RoleModel?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<RoleModel?> ReadAsync(Guid? id = null, string? uniqueName = null, IRequestContext? context = null);
  Task<RoleModel?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<RoleModel>> SearchAsync(SearchRolesPayload payload, IRequestContext? context = null);
  Task<RoleModel?> UpdateAsync(Guid id, UpdateRolePayload payload, IRequestContext? context = null);
}
