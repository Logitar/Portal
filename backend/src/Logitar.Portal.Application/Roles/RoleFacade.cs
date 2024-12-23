using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Roles.Commands;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Roles;
internal class RoleFacade : IRoleService
{
  private readonly IActivityPipeline _activityPipeline;

  public RoleFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<RoleModel> CreateAsync(CreateRolePayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateRoleCommand(payload), cancellationToken);
  }

  public async Task<RoleModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteRoleCommand(id), cancellationToken);
  }

  public async Task<RoleModel?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadRoleQuery(id, uniqueName), cancellationToken);
  }

  public async Task<RoleModel?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReplaceRoleCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<RoleModel>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchRolesQuery(payload), cancellationToken);
  }

  public async Task<RoleModel?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new UpdateRoleCommand(id, payload), cancellationToken);
  }
}
