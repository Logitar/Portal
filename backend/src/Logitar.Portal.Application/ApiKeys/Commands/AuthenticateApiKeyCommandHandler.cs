using FluentValidation;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Portal.Application.ApiKeys.Validators;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class AuthenticateApiKeyCommandHandler : IRequestHandler<AuthenticateApiKeyCommand, ApiKey>
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApplicationContext _applicationContext;

  public AuthenticateApiKeyCommandHandler(IApiKeyRepository apiKeyRepository, IApiKeyQuerier apiKeyQuerier, IApplicationContext applicationContext)
  {
    _apiKeyRepository = apiKeyRepository;
    _apiKeyQuerier = apiKeyQuerier;
    _applicationContext = applicationContext;
  }

  public async Task<ApiKey> Handle(AuthenticateApiKeyCommand command, CancellationToken cancellationToken)
  {
    AuthenticateApiKeyPayload payload = command.Payload;
    new AuthenticateApiKeyValidator().ValidateAndThrow(payload);

    XApiKey xApiKey;
    try
    {
      xApiKey = XApiKey.Decode(payload.ApiKey);
    }
    catch (Exception innerException)
    {
      throw new InvalidApiKeyException(payload.ApiKey, nameof(payload.ApiKey), innerException);
    }

    ApiKeyAggregate apiKey = await _apiKeyRepository.LoadAsync(xApiKey.Id, cancellationToken)
      ?? throw new ApiKeyNotFoundException(xApiKey.Id, nameof(payload.ApiKey));

    apiKey.Authenticate(xApiKey.Secret, _applicationContext.ActorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }
}
