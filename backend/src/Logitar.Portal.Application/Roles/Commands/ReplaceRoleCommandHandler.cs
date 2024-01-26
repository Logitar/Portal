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

internal class ReplaceRoleCommandHandler : IRequestHandler<ReplaceRoleCommand, Role?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public ReplaceRoleCommandHandler(IApplicationContext applicationContext, IRoleManager roleManager, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _roleManager = roleManager;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<Role?> Handle(ReplaceRoleCommand command, CancellationToken cancellationToken)
  {
    IRoleSettings roleSettings = _applicationContext.RoleSettings;

    ReplaceRolePayload payload = command.Payload;
    new ReplaceRoleValidator(roleSettings).ValidateAndThrow(payload);

    RoleId roleId = new(command.Id);
    RoleAggregate? role = await _roleRepository.LoadAsync(roleId, cancellationToken);
    if (role == null)
    {
      return null;
    }
    // TODO(fpion): ensure Role is in current Realm

    RoleAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _roleRepository.LoadAsync(roleId, command.Version.Value, cancellationToken);
    }

    ActorId actorId = _applicationContext.ActorId;

    UniqueNameUnit uniqueName = new(roleSettings.UniqueName, payload.UniqueName);
    if (reference == null || uniqueName != reference.UniqueName)
    {
      role.SetUniqueName(uniqueName, actorId);
    }
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      role.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      role.Description = description;
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      // TODO(fpion): implement
    }

    role.Update(actorId);

    await _roleManager.SaveAsync(role, actorId, cancellationToken);

    return await _roleQuerier.ReadAsync(role, cancellationToken);
  }
}
