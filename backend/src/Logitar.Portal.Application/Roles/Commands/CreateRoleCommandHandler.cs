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

internal class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Role>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;

  public CreateRoleCommandHandler(IApplicationContext applicationContext, IRoleManager roleManager, IRoleQuerier roleQuerier)
  {
    _applicationContext = applicationContext;
    _roleManager = roleManager;
    _roleQuerier = roleQuerier;
  }

  public async Task<Role> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
  {
    IRoleSettings roleSettings = _applicationContext.RoleSettings;

    CreateRolePayload payload = command.Payload;
    new CreateRoleValidator(roleSettings).ValidateAndThrow(payload);

    UniqueNameUnit uniqueName = new(roleSettings.UniqueName, payload.UniqueName);
    ActorId actorId = _applicationContext.ActorId;
    RoleAggregate role = new(uniqueName, _applicationContext.TenantId, actorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      role.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    role.Update(actorId);

    await _roleManager.SaveAsync(role, actorId, cancellationToken);

    return await _roleQuerier.ReadAsync(role, cancellationToken);
  }
}
