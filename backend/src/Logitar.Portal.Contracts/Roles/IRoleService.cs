namespace Logitar.Portal.Contracts.Roles;

public interface IRoleService
{
  Task<Role> CreateAsync(CreateRolePayload payload, CancellationToken cancellationToken = default);
  Task<Role?> DeleteAsync(string id, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(string? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<Role?> ReplaceAsync(string id, ReplaceRolePayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<Role?> UpdateAsync(string id, UpdateRolePayload payload, CancellationToken cancellationToken = default);
}
