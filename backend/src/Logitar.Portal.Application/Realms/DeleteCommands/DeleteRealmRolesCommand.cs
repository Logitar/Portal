﻿using Logitar.EventSourcing;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmRolesCommand(Realm Realm, ActorId ActorId) : INotification;

internal class DeleteRealmRolesCommandHandler : INotificationHandler<DeleteRealmRolesCommand>
{
  private readonly IRoleRepository _roleRepository;

  public DeleteRealmRolesCommandHandler(IRoleRepository roleRepository)
  {
    _roleRepository = roleRepository;
  }

  public async Task Handle(DeleteRealmRolesCommand command, CancellationToken cancellationToken)
  {
    TenantId tenantId = new(command.Realm.Id.Value);
    IEnumerable<RoleAggregate> roles = await _roleRepository.LoadAsync(tenantId, cancellationToken);

    foreach (RoleAggregate role in roles)
    {
      role.Delete(command.ActorId);
    }

    await _roleRepository.SaveAsync(roles, cancellationToken);
  }
}
