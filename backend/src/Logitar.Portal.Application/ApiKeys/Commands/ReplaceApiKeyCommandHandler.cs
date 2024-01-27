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

internal class ReplaceApiKeyCommandHandler : IRequestHandler<ReplaceApiKeyCommand, ApiKey?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;

  public ReplaceApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IApplicationContext applicationContext, IMediator mediator)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _mediator = mediator;
  }

  public async Task<ApiKey?> Handle(ReplaceApiKeyCommand command, CancellationToken cancellationToken)
  {
    ReplaceApiKeyPayload payload = command.Payload;
    new ReplaceApiKeyValidator().ValidateAndThrow(payload);

    ApiKeyId apiKeyId = new(command.Id);
    ApiKeyAggregate? apiKey = await _apiKeyRepository.LoadAsync(apiKeyId, cancellationToken);
    if (apiKey == null)
    {
      return null;
    }
    apiKey.EnsureIsInRealm(_applicationContext);

    ApiKeyAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _apiKeyRepository.LoadAsync(apiKeyId, command.Version.Value, cancellationToken);
    }

    ActorId actorId = _applicationContext.ActorId;

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

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      // TODO(fpion): implement
    }

    IEnumerable<RoleAggregate> roles = await _mediator.Send(new FindRolesQuery(apiKey.TenantId, payload.Roles, nameof(payload.Roles)), cancellationToken);
    // TODO(fpion): implement

    apiKey.Update(actorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }
}
