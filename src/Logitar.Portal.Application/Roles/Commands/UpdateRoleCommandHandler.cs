using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Role?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public UpdateRoleCommandHandler(IApplicationContext applicationContext, IRealmRepository realmRepository, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<Role?> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
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

    UpdateRolePayload payload = command.Payload;

    if (payload.UniqueName != null)
    {
      RoleAggregate? other = await _roleRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken);
      if (other?.Equals(role) == false)
      {
        throw new UniqueNameAlreadyUsedException<RoleAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
      }

      IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;
      role.SetUniqueName(uniqueNameSettings, payload.UniqueName);
    }
    if (payload.DisplayName != null)
    {
      role.DisplayName = payload.DisplayName.Value;
    }
    if (payload.Description != null)
    {
      role.Description = payload.Description.Value;
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

    role.Update(_applicationContext.ActorId);

    await _roleRepository.SaveAsync(role, cancellationToken);

    return await _roleQuerier.ReadAsync(role, cancellationToken);
  }
}
