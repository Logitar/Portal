﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.ApiKeys.Validators;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record CreateApiKeyCommand(CreateApiKeyPayload Payload) : Activity, IRequest<ApiKeyModel>;

internal class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, ApiKeyModel>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;

  public CreateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IMediator mediator, IPasswordManager passwordManager)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _mediator = mediator;
    _passwordManager = passwordManager;
  }

  public async Task<ApiKeyModel> Handle(CreateApiKeyCommand command, CancellationToken cancellationToken)
  {
    CreateApiKeyPayload payload = command.Payload;
    new CreateApiKeyValidator().ValidateAndThrow(payload);

    ActorId actorId = command.ActorId;

    DisplayName displayName = new(payload.DisplayName);
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out string secretString);
    ApiKey apiKey = new(displayName, secret, actorId, ApiKeyId.NewId(command.TenantId))
    {
      Description = Description.TryCreate(payload.Description)
    };
    if (payload.ExpiresOn.HasValue)
    {
      apiKey.ExpiresOn = payload.ExpiresOn.Value;
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      apiKey.SetCustomAttribute(key, customAttribute.Value);
    }

    IEnumerable<FoundRole> roles = await _mediator.Send(new FindRolesQuery(apiKey.TenantId, payload.Roles, nameof(payload.Roles)), cancellationToken);
    foreach (FoundRole found in roles)
    {
      apiKey.AddRole(found.Role, actorId);
    }

    apiKey.Update(actorId);
    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    ApiKeyModel result = await _apiKeyQuerier.ReadAsync(command.Realm, apiKey, cancellationToken);
    result.XApiKey = XApiKey.Encode(apiKey.Id, secretString);
    return result;
  }
}
