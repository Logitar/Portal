using Logitar.Portal.Application.Roles.Commands;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Roles;
internal class RoleFacade : IRoleService
{
  private readonly IMediator _mediator;

  public RoleFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Role> CreateAsync(CreateRolePayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateRoleCommand(payload), cancellationToken);
  }

  public async Task<Role?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteRoleCommand(id), cancellationToken);
  }

  public async Task<Role?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadRoleQuery(id, uniqueName), cancellationToken);
  }

  public async Task<Role?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReplaceRoleCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SearchRolesQuery(payload), cancellationToken);
  }

  public async Task<Role?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new UpdateRoleCommand(id, payload), cancellationToken);
  }
}
