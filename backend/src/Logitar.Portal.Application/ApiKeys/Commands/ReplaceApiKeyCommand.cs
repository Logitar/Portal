using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;
using Logitar.Identity.Core.Roles;
using Logitar.Identity.Core.Settings;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.ApiKeys.Validators;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record ReplaceApiKeyCommand(Guid Id, ReplaceApiKeyPayload Payload, long? Version) : Activity, IRequest<ApiKeyModel?>;

internal class ReplaceApiKeyCommandHandler : IRequestHandler<ReplaceApiKeyCommand, ApiKeyModel?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IMediator _mediator;

  public ReplaceApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IMediator mediator)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _mediator = mediator;
  }

  public async Task<ApiKeyModel?> Handle(ReplaceApiKeyCommand command, CancellationToken cancellationToken)
  {
    ReplaceApiKeyPayload payload = command.Payload;
    new ReplaceApiKeyValidator().ValidateAndThrow(payload);

    ApiKeyId apiKeyId = new(command.TenantId, new EntityId(command.Id));
    ApiKey? apiKey = await _apiKeyRepository.LoadAsync(apiKeyId, cancellationToken);
    if (apiKey == null || apiKey.TenantId != command.TenantId)
    {
      return null;
    }
    ApiKey? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _apiKeyRepository.LoadAsync(apiKey.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = command.ActorId;

    DisplayName displayName = new(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      apiKey.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      apiKey.Description = description;
    }
    if (payload.ExpiresOn.HasValue && (reference == null || payload.ExpiresOn != reference.ExpiresOn))
    {
      apiKey.ExpiresOn = payload.ExpiresOn.Value;
    }

    ReplaceCustomAttributes(payload, apiKey, reference);
    await ReplaceRolesAsync(payload, apiKey, reference, actorId, cancellationToken);

    apiKey.Update(actorId);
    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(command.Realm, apiKey, cancellationToken);
  }

  private static void ReplaceCustomAttributes(ReplaceApiKeyPayload payload, ApiKey apiKey, ApiKey? reference)
  {
    HashSet<Identifier> payloadKeys = new(capacity: payload.CustomAttributes.Count);

    IEnumerable<Identifier> referenceKeys;
    if (reference == null)
    {
      referenceKeys = apiKey.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        Identifier key = new(customAttribute.Key);
        payloadKeys.Add(key);
        apiKey.SetCustomAttribute(key, customAttribute.Value);
      }
    }
    else
    {
      referenceKeys = reference.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        Identifier key = new(customAttribute.Key);
        payloadKeys.Add(key);

        string value = customAttribute.Value.Trim();
        if (!reference.CustomAttributes.TryGetValue(key, out string? existingValue) || value != existingValue)
        {
          apiKey.SetCustomAttribute(key, value);
        }
      }
    }

    foreach (Identifier key in referenceKeys)
    {
      if (!payloadKeys.Contains(key))
      {
        apiKey.RemoveCustomAttribute(key);
      }
    }
  }

  private async Task ReplaceRolesAsync(ReplaceApiKeyPayload payload, ApiKey apiKey, ApiKey? reference, ActorId actorId, CancellationToken cancellationToken)
  {
    IEnumerable<FoundRole> roles = await _mediator.Send(new FindRolesQuery(apiKey.TenantId, payload.Roles, nameof(payload.Roles)), cancellationToken);
    HashSet<RoleId> roleIds = new(capacity: roles.Count());

    IEnumerable<RoleId> referenceRoles;
    if (reference == null)
    {
      referenceRoles = apiKey.Roles;

      foreach (FoundRole found in roles)
      {
        roleIds.Add(found.Role.Id);
        apiKey.AddRole(found.Role, actorId);
      }
    }
    else
    {
      referenceRoles = reference.Roles;

      foreach (FoundRole found in roles)
      {
        if (!reference.HasRole(found.Role))
        {
          roleIds.Add(found.Role.Id);
          apiKey.AddRole(found.Role, actorId);
        }
      }
    }

    foreach (RoleId roleId in referenceRoles)
    {
      if (!roleIds.Contains(roleId))
      {
        Role role = new(new UniqueName(new UniqueNameSettings(), "TEMP"), actorId: null, roleId);
        apiKey.RemoveRole(role, actorId);
      }
    }
  }
}
