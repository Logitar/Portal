using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.ApiKeys.Validators;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record UpdateApiKeyCommand(Guid Id, UpdateApiKeyPayload Payload) : Activity, IRequest<ApiKeyModel?>;

internal class UpdateApiKeyCommandHandler : IRequestHandler<UpdateApiKeyCommand, ApiKeyModel?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IMediator _mediator;

  public UpdateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IMediator mediator)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _mediator = mediator;
  }

  public async Task<ApiKeyModel?> Handle(UpdateApiKeyCommand command, CancellationToken cancellationToken)
  {
    UpdateApiKeyPayload payload = command.Payload;
    new UpdateApiKeyValidator().ValidateAndThrow(payload);

    ApiKeyAggregate? apiKey = await _apiKeyRepository.LoadAsync(command.Id, cancellationToken);
    if (apiKey == null || apiKey.TenantId != command.TenantId)
    {
      return null;
    }

    ActorId actorId = command.ActorId;

    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    if (displayName != null)
    {
      apiKey.DisplayName = displayName;
    }
    if (payload.Description != null)
    {
      apiKey.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }
    if (payload.ExpiresOn.HasValue)
    {
      apiKey.SetExpiration(payload.ExpiresOn.Value);
    }

    foreach (CustomAttributeModification customAttribute in payload.CustomAttributes)
    {
      if (string.IsNullOrWhiteSpace(customAttribute.Value))
      {
        apiKey.RemoveCustomAttribute(customAttribute.Key);
      }
      else
      {
        apiKey.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }

    IReadOnlyCollection<FoundRole> roles = await _mediator.Send(new FindRolesQuery(apiKey.TenantId, payload.Roles, nameof(payload.Roles)), cancellationToken);
    foreach (FoundRole found in roles)
    {
      switch (found.Action)
      {
        case CollectionAction.Add:
          apiKey.AddRole(found.Role, actorId);
          break;
        case CollectionAction.Remove:
          apiKey.RemoveRole(found.Role, actorId);
          break;
      }
    }

    apiKey.Update(actorId);
    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(command.Realm, apiKey, cancellationToken);
  }
}
