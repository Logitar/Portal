using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class UpdateApiKeyCommandHandler : IRequestHandler<UpdateApiKeyCommand, ApiKey?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly IRoleRepository _roleRepository;

  public UpdateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository,
    IApplicationContext applicationContext, IRealmRepository realmRepository, IRoleRepository roleRepository)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _roleRepository = roleRepository;
  }

  public async Task<ApiKey?> Handle(UpdateApiKeyCommand command, CancellationToken cancellationToken)
  {
    ApiKeyAggregate? apiKey = await _apiKeyRepository.LoadAsync(command.Id, cancellationToken);
    if (apiKey == null)
    {
      return null;
    }

    RealmAggregate? realm = null;
    if (apiKey.TenantId != null)
    {
      realm = await _realmRepository.LoadAsync(apiKey, cancellationToken);
    }

    UpdateApiKeyPayload payload = command.Payload;

    if (payload.Title != null)
    {
      apiKey.Title = payload.Title;
    }
    if (payload.Description != null)
    {
      apiKey.Description = payload.Description.Value;
    }
    if (payload.ExpiresOn.HasValue)
    {
      apiKey.ExpiresOn = payload.ExpiresOn.Value;
    }

    foreach (CustomAttributeModification customAttribute in payload.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        apiKey.RemoveCustomAttribute(customAttribute.Key);
      }
      else
      {
        apiKey.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }

    await SetRolesAsync(apiKey, payload, realm, cancellationToken);

    apiKey.Update(_applicationContext.ActorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }

  private async Task SetRolesAsync(ApiKeyAggregate apiKey, UpdateApiKeyPayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    int roleCount = payload.Roles.Count();
    if (roleCount > 0)
    {
      IEnumerable<RoleAggregate> roles = await _roleRepository.LoadAsync(realm, cancellationToken);
      Dictionary<Guid, RoleAggregate> rolesById = roles.ToDictionary(r => r.Id.ToGuid(), r => r);
      Dictionary<string, RoleAggregate> rolesByUniqueName = roles.ToDictionary(r => r.UniqueName.ToUpper(), r => r);

      List<string> missingRoles = new(capacity: roleCount);

      foreach (RoleModification roleAction in payload.Roles)
      {
        string roleId = roleAction.Role.Trim();
        string uniqueName = roleId.ToUpper();

        if ((Guid.TryParse(roleId, out Guid id) && rolesById.TryGetValue(id, out RoleAggregate? role))
          || rolesByUniqueName.TryGetValue(uniqueName.Trim().ToUpper(), out role))
        {
          switch (roleAction.Action)
          {
            case CollectionAction.Add:
              apiKey.AddRole(role);
              break;
            case CollectionAction.Remove:
              apiKey.RemoveRole(role);
              break;
          }
        }
        else
        {
          missingRoles.Add(roleAction.Role);
        }
      }

      if (missingRoles.Any())
      {
        throw new RolesNotFoundException(missingRoles, nameof(payload.Roles));
      }
    }
  }
}
