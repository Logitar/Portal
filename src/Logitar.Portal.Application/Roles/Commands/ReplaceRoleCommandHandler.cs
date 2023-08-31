using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal class ReplaceRoleCommandHandler : IRequestHandler<ReplaceRoleCommand, Role?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public ReplaceRoleCommandHandler(IApplicationContext applicationContext, IRealmRepository realmRepository, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<Role?> Handle(ReplaceRoleCommand command, CancellationToken cancellationToken)
  {
    RoleAggregate? role = await _roleRepository.LoadAsync(command.Id, cancellationToken);
    if (role == null)
    {
      return null;
    }

    RealmAggregate? realm = null;
    if (role.TenantId != null)
    {
      realm = await _realmRepository.LoadAsync(role, cancellationToken);
    }
    string? tenantId = realm?.Id.Value;

    RoleAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _roleRepository.LoadAsync(role.Id, command.Version.Value, cancellationToken);
    }

    ReplaceRolePayload payload = command.Payload;

    if (reference == null || (payload.UniqueName.Trim() != reference.UniqueName))
    {
      RoleAggregate? other = await _roleRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken);
      if (other?.Equals(role) == false)
      {
        throw new UniqueNameAlreadyUsedException<RoleAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
      }

      IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;
      role.SetUniqueName(uniqueNameSettings, payload.UniqueName);
    }
    if (reference == null || (payload.DisplayName?.CleanTrim() != reference.DisplayName))
    {
      role.DisplayName = payload.DisplayName;
    }
    if (reference == null || (payload.Description?.CleanTrim() != reference.Description))
    {
      role.Description = payload.Description;
    }

    HashSet<string> customAttributeKeys = payload.CustomAttributes.Select(x => x.Key.Trim()).ToHashSet();
    foreach (string key in role.CustomAttributes.Keys)
    {
      if (!customAttributeKeys.Contains(key) && (reference == null || reference.CustomAttributes.ContainsKey(key)))
      {
        role.RemoveCustomAttribute(key);
      }
    }
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      role.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    role.Update(_applicationContext.ActorId);

    await _roleRepository.SaveAsync(role, cancellationToken);

    return await _roleQuerier.ReadAsync(role, cancellationToken);
  }
}
