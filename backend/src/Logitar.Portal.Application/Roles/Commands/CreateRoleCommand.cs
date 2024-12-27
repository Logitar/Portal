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

internal record CreateRoleCommand(CreateRolePayload Payload) : Activity, IRequest<RoleModel>;

internal class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleModel>
{
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;

  public CreateRoleCommandHandler(IRoleManager roleManager, IRoleQuerier roleQuerier)
  {
    _roleManager = roleManager;
    _roleQuerier = roleQuerier;
  }

  public async Task<RoleModel> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
  {
    IRoleSettings roleSettings = command.RoleSettings;

    CreateRolePayload payload = command.Payload;
    new CreateRoleValidator(roleSettings).ValidateAndThrow(payload);

    ActorId actorId = command.ActorId;

    UniqueName uniqueName = new(roleSettings.UniqueName, payload.UniqueName);
    Role role = new(uniqueName, actorId, RoleId.NewId(command.TenantId))
    {
      DisplayName = DisplayName.TryCreate(payload.DisplayName),
      Description = Description.TryCreate(payload.Description)
    };
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      role.SetCustomAttribute(key, customAttribute.Value);
    }
    role.Update(actorId);

    await _roleManager.SaveAsync(role, actorId, cancellationToken);

    return await _roleQuerier.ReadAsync(command.Realm, role, cancellationToken);
  }
}
