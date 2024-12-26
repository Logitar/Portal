using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.ApiKeys.Validators;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class ReplaceApiKeyCommandHandler : IRequestHandler<ReplaceApiKeyCommand, ApiKey?>
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

  public async Task<ApiKey?> Handle(ReplaceApiKeyCommand command, CancellationToken cancellationToken)
  {
    ReplaceApiKeyPayload payload = command.Payload;
    new ReplaceApiKeyValidator().ValidateAndThrow(payload);

    ApiKeyAggregate? apiKey = await _apiKeyRepository.LoadAsync(command.Id, cancellationToken);
    if (apiKey == null || apiKey.TenantId != command.TenantId)
    {
      return null;
    }
    ApiKeyAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _apiKeyRepository.LoadAsync(apiKey.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = command.ActorId;

    DisplayNameUnit displayName = new(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      apiKey.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      apiKey.Description = description;
    }
    if (payload.ExpiresOn.HasValue && (reference == null || payload.ExpiresOn != reference.ExpiresOn))
    {
      apiKey.SetExpiration(payload.ExpiresOn.Value);
    }

    ReplaceCustomAttributes(payload, apiKey, reference);
    await ReplaceRolesAsync(payload, apiKey, reference, actorId, cancellationToken);

    apiKey.Update(actorId);
    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(command.Realm, apiKey, cancellationToken);
  }

  private static void ReplaceCustomAttributes(ReplaceApiKeyPayload payload, ApiKeyAggregate apiKey, ApiKeyAggregate? reference)
  {
    HashSet<string> payloadKeys = new(capacity: payload.CustomAttributes.Count);

    IEnumerable<string> referenceKeys;
    if (reference == null)
    {
      referenceKeys = apiKey.CustomAttributes.Keys;

      foreach (CustomAttribute customAttribute in payload.CustomAttributes)
      {
        payloadKeys.Add(customAttribute.Key.Trim());
        apiKey.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
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
          apiKey.SetCustomAttribute(key, value);
        }
      }
    }

    foreach (string key in referenceKeys)
    {
      if (!payloadKeys.Contains(key))
      {
        apiKey.RemoveCustomAttribute(key);
      }
    }
  }

  private async Task ReplaceRolesAsync(ReplaceApiKeyPayload payload, ApiKeyAggregate apiKey, ApiKeyAggregate? reference, ActorId actorId, CancellationToken cancellationToken)
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
        RoleAggregate role = new(roleId.AggregateId);
        apiKey.RemoveRole(role, actorId);
      }
    }
  }
}
