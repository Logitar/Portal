using Logitar.EventSourcing;
using Logitar.Identity.Domain.Roles;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Role?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public DeleteRoleCommandHandler(IApplicationContext applicationContext,
    IRoleManager roleManager, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _roleManager = roleManager;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<Role?> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
  {
    RoleAggregate? role = await _roleRepository.LoadAsync(command.Id, cancellationToken);
    if (role == null || role.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    Role result = await _roleQuerier.ReadAsync(role, cancellationToken);

    ActorId actorId = _applicationContext.ActorId;
    role.Delete(actorId);
    await _roleManager.SaveAsync(role, actorId, cancellationToken);

    return result;
  }
}
