using Logitar.EventSourcing;
using Logitar.Identity.Domain.Roles;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Role?>
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

  public async Task<Role?> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
  {
    RoleAggregate? role = await _roleRepository.LoadAsync(command.Id, cancellationToken);
    if (role == null || role.TenantId != command.TenantId)
    {
      return null;
    }
    Role result = await _roleQuerier.ReadAsync(command.Realm, role, cancellationToken);

    ActorId actorId = command.ActorId;
    role.Delete(actorId);
    await _roleManager.SaveAsync(role, actorId, cancellationToken);

    return result;
  }
}
