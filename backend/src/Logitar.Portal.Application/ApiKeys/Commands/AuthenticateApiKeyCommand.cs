using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core.ApiKeys;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.ApiKeys.Validators;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

public record AuthenticateApiKeyCommand(AuthenticateApiKeyPayload Payload) : Activity, IRequest<ApiKeyModel>
{
  public override IActivity Anonymize()
  {
    AuthenticateApiKeyCommand command = this.DeepClone();
    command.Payload.XApiKey = Payload.XApiKey.Mask();
    return command;
  }
}

internal class AuthenticateApiKeyCommandHandler : IRequestHandler<AuthenticateApiKeyCommand, ApiKeyModel>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;

  public AuthenticateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
  }

  public async Task<ApiKeyModel> Handle(AuthenticateApiKeyCommand command, CancellationToken cancellationToken)
  {
    AuthenticateApiKeyPayload payload = command.Payload;
    new AuthenticateApiKeyValidator().ValidateAndThrow(payload);

    XApiKey xApiKey;
    try
    {
      xApiKey = XApiKey.Decode(command.TenantId, payload.XApiKey);
    }
    catch (Exception innerException)
    {
      throw new InvalidApiKeyException(payload.XApiKey, nameof(payload.XApiKey), innerException);
    }

    ApiKey apiKey = await _apiKeyRepository.LoadAsync(xApiKey.Id, cancellationToken)
      ?? throw new ApiKeyNotFoundException(xApiKey.Id, nameof(payload.XApiKey));
    if (apiKey.TenantId != command.TenantId)
    {
      throw new ApiKeyNotFoundException(apiKey.Id, nameof(payload.XApiKey));
    }

    ActorId? actorId = command.Actor.Type == ActorType.System ? null : command.ActorId;
    apiKey.Authenticate(xApiKey.Secret, actorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(command.Realm, apiKey, cancellationToken);
  }
}
