using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Role?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPublisher _publisher;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public DeleteRoleCommandHandler(IApplicationContext applicationContext, IPublisher publisher,
    IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _publisher = publisher;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<Role?> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
  {
    RoleAggregate? role = await _roleRepository.LoadAsync(command.Id, cancellationToken);
    if (role == null)
    {
      return null;
    }
    Role result = await _roleQuerier.ReadAsync(role, cancellationToken);

    await _publisher.Publish(new RemoveRolesCommand(role), cancellationToken);

    role.Delete(_applicationContext.ActorId);

    await _roleRepository.SaveAsync(role, cancellationToken);

    return result;
  }
}
