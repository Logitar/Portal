using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Roles;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Roles.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record UpdateRoleCommand(Guid Id, UpdateRolePayload Payload) : Activity, IRequest<RoleModel?>;

internal class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleModel?>
{
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public UpdateRoleCommandHandler(IRoleManager roleManager, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _roleManager = roleManager;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<RoleModel?> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
  {
    IRoleSettings roleSettings = command.RoleSettings;

    UpdateRolePayload payload = command.Payload;
    new UpdateRoleValidator(roleSettings).ValidateAndThrow(payload);

    RoleId roleId = new(command.TenantId, new EntityId(command.Id));
    Role? role = await _roleRepository.LoadAsync(roleId, cancellationToken);
    if (role == null || role.TenantId != command.TenantId)
    {
      return null;
    }

    ActorId actorId = command.ActorId;

    UniqueName? uniqueName = UniqueName.TryCreate(payload.UniqueName, roleSettings.UniqueName);
    if (uniqueName != null)
    {
      role.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName != null)
    {
      role.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      role.Description = Description.TryCreate(payload.Description.Value);
    }

    foreach (CustomAttributeModification customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      if (string.IsNullOrWhiteSpace(customAttribute.Value))
      {
        role.RemoveCustomAttribute(key);
      }
      else
      {
        role.SetCustomAttribute(key, customAttribute.Value);
      }
    }

    role.Update(actorId);
    await _roleManager.SaveAsync(role, actorId, cancellationToken);

    return await _roleQuerier.ReadAsync(command.Realm, role, cancellationToken);
  }
}
