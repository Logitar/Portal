using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal class DeleteRolesCommandHandler : INotificationHandler<DeleteRolesCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleRepository _roleRepository;

  public DeleteRolesCommandHandler(IApplicationContext applicationContext, IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _roleRepository = roleRepository;
  }

  public async Task Handle(DeleteRolesCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<RoleAggregate> roles = await _roleRepository.LoadAsync(command.Realm, cancellationToken);
    foreach (RoleAggregate role in roles)
    {
      role.Delete(_applicationContext.ActorId);
    }

    await _roleRepository.SaveAsync(roles, cancellationToken);
  }
}
