using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Roles;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record DeleteRoleCommand(Guid Id) : Activity, IRequest<RoleModel?>;

internal class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, RoleModel?>
{
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public DeleteRoleCommandHandler(IRoleManager roleManager, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _roleManager = roleManager;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<RoleModel?> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
  {
    RoleId roleId = new(command.TenantId, new EntityId(command.Id));
    Role? role = await _roleRepository.LoadAsync(roleId, cancellationToken);
    if (role == null || role.TenantId != command.TenantId)
    {
      return null;
    }
    RoleModel result = await _roleQuerier.ReadAsync(command.Realm, role, cancellationToken);

    ActorId actorId = command.ActorId;
    role.Delete(actorId);
    await _roleManager.SaveAsync(role, actorId, cancellationToken);

    return result;
  }
}
