using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Roles.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record ReplaceRoleCommand(Guid Id, ReplaceRolePayload Payload, long? Version) : Activity, IRequest<RoleModel?>;

internal class ReplaceRoleCommandHandler : IRequestHandler<ReplaceRoleCommand, RoleModel?>
{
  private readonly IRoleManager _roleManager;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public ReplaceRoleCommandHandler(IRoleManager roleManager, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _roleManager = roleManager;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<RoleModel?> Handle(ReplaceRoleCommand command, CancellationToken cancellationToken)
  {
    IRoleSettings roleSettings = command.RoleSettings;

    ReplaceRolePayload payload = command.Payload;
    new ReplaceRoleValidator(roleSettings).ValidateAndThrow(payload);

    RoleAggregate? role = await _roleRepository.LoadAsync(command.Id, cancellationToken);
    if (role == null || role.TenantId != command.TenantId)
    {
      return null;
    }
    RoleAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _roleRepository.LoadAsync(role.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = command.ActorId;

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

    ReplaceCustomAttributes(payload, role, reference);

    role.Update(actorId);
    await _roleManager.SaveAsync(role, actorId, cancellationToken);

    return await _roleQuerier.ReadAsync(command.Realm, role, cancellationToken);
  }

  private static void ReplaceCustomAttributes(ReplaceRolePayload payload, RoleAggregate role, RoleAggregate? reference)
  {
    HashSet<string> payloadKeys = new(capacity: payload.CustomAttributes.Count);

    IEnumerable<string> referenceKeys;
    if (reference == null)
    {
      referenceKeys = role.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        payloadKeys.Add(customAttribute.Key.Trim());
        role.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }
    else
    {
      referenceKeys = reference.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        string key = customAttribute.Key.Trim();
        payloadKeys.Add(key);

        string value = customAttribute.Value.Trim();
        if (!reference.CustomAttributes.TryGetValue(key, out string? existingValue) || existingValue != value)
        {
          role.SetCustomAttribute(key, value);
        }
      }
    }

    foreach (string key in referenceKeys)
    {
      if (!payloadKeys.Contains(key))
      {
        role.RemoveCustomAttribute(key);
      }
    }
  }
}
