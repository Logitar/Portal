using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Roles;

public interface IRoleClient
{
  Task<Role> CreateAsync(CreateRolePayload payload, IRequestContext? context = null);
  Task<Role?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<Role?> ReadAsync(Guid? id = null, string? uniqueName = null, IRequestContext? context = null);
  Task<Role?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, IRequestContext? context = null);
  Task<Role?> UpdateAsync(Guid id, UpdateRolePayload payload, IRequestContext? context = null);
}
