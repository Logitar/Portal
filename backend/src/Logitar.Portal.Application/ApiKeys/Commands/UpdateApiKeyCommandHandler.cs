using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.ApiKeys.Validators;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class UpdateApiKeyCommandHandler : IRequestHandler<UpdateApiKeyCommand, ApiKey?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;

  public UpdateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IApplicationContext applicationContext, IMediator mediator)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _mediator = mediator;
  }

  public async Task<ApiKey?> Handle(UpdateApiKeyCommand command, CancellationToken cancellationToken)
  {
    UpdateApiKeyPayload payload = command.Payload;
    new UpdateApiKeyValidator().ValidateAndThrow(payload);

    ApiKeyId apiKeyId = new(command.Id);
    ApiKeyAggregate? apiKey = await _apiKeyRepository.LoadAsync(apiKeyId, cancellationToken);
    if (apiKey == null)
    {
      return null;
    }
    apiKey.EnsureIsInRealm(_applicationContext);

    ActorId actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.DisplayName))
    {
      apiKey.DisplayName = new DisplayNameUnit(payload.DisplayName);
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
      if (customAttribute.Value == null)
      {
        apiKey.RemoveCustomAttribute(customAttribute.Key);
      }
      else
      {
        apiKey.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
    }

    IEnumerable<string> roleIds = payload.Roles.Select(x => x.Role);
    IEnumerable<RoleAggregate> roles = await _mediator.Send(new FindRolesQuery(apiKey.TenantId, roleIds, nameof(payload.Roles)), cancellationToken);

    apiKey.Update(actorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }
}
