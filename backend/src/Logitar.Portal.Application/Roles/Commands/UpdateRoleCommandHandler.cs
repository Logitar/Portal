using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Roles.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Role?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public UpdateRoleCommandHandler(IApplicationContext applicationContext, IRoleManager roleManager, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _roleManager = roleManager;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<Role?> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
  {
    IRoleSettings roleSettings = _applicationContext.RoleSettings;

    UpdateRolePayload payload = command.Payload;
    new UpdateRoleValidator(roleSettings).ValidateAndThrow(payload);

    RoleId roleId = new(command.Id);
    RoleAggregate? role = await _roleRepository.LoadAsync(roleId, cancellationToken);
    if (role == null)
    {
      return null;
    }
    // TODO(fpion): ensure Role is in current Realm

    ActorId actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueNameUnit uniqueName = new(roleSettings.UniqueName, payload.UniqueName);
      role.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName != null)
    {
      role.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      role.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    foreach (CustomAttributeModification customAttribute in payload.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        role.RemoveCustomAttribute(customAttribute.Key);
      }
      else
      {
        role.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }

    role.Update(actorId);

    await _roleManager.SaveAsync(role, actorId, cancellationToken);

    return await _roleQuerier.ReadAsync(role, cancellationToken);
  }
}
