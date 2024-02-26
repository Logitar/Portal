using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Portal.Application.ApiKeys.Validators;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class AuthenticateApiKeyCommandHandler : IRequestHandler<AuthenticateApiKeyCommand, ApiKey>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;

  public AuthenticateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier,
    IApiKeyRepository apiKeyRepository, IApplicationContext applicationContext)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
  }

  public async Task<ApiKey> Handle(AuthenticateApiKeyCommand command, CancellationToken cancellationToken)
  {
    AuthenticateApiKeyPayload payload = command.Payload;
    new AuthenticateApiKeyValidator().ValidateAndThrow(payload);

    XApiKey xApiKey;
    try
    {
      xApiKey = XApiKey.Decode(payload.XApiKey);
    }
    catch (Exception innerException)
    {
      throw new InvalidApiKeyException(payload.XApiKey, nameof(payload.XApiKey), innerException);
    }

    ApiKeyAggregate apiKey = await _apiKeyRepository.LoadAsync(xApiKey.Id, cancellationToken)
      ?? throw new ApiKeyNotFoundException(xApiKey.Id, nameof(payload.XApiKey));
    if (apiKey.TenantId != _applicationContext.TenantId)
    {
      throw new ApiKeyNotFoundException(apiKey.Id, nameof(payload.XApiKey));
    }

    ActorId? actorId = _applicationContext.Actor.Type == ActorType.System ? null : _applicationContext.ActorId;
    apiKey.Authenticate(xApiKey.Secret, actorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }
}
