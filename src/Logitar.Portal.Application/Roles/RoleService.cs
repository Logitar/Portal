using Logitar.Portal.Application.Roles.Commands;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Application.Roles;

internal class RoleService : IRoleService
{
  private readonly IRequestPipeline _pipeline;

  public RoleService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Role> CreateAsync(CreateRolePayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateRoleCommand(payload), cancellationToken);
  }

  public async Task<Role?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteRoleCommand(id), cancellationToken);
  }

  public async Task<Role?> ReadAsync(Guid? id, string? realm, string? uniqueName, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadRoleQuery(id, realm, uniqueName), cancellationToken);
  }

  public async Task<Role?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReplaceRoleCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SearchRolesQuery(payload), cancellationToken);
  }

  public async Task<Role?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateRoleCommand(id, payload), cancellationToken);
  }
}
