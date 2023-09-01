using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class AuthenticateApiKeyCommandHandler : IRequestHandler<AuthenticateApiKeyCommand, ApiKey>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;

  public AuthenticateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IApplicationContext applicationContext)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
  }

  public async Task<ApiKey> Handle(AuthenticateApiKeyCommand command, CancellationToken cancellationToken)
  {
    XApiKey xApiKey;
    try
    {
      xApiKey = XApiKey.Decode(command.XApiKey);
    }
    catch (Exception innerException)
    {
      throw new InvalidXApiKeyException(command.XApiKey, innerException);
    }

    ApiKeyAggregate apiKey = await _apiKeyRepository.LoadAsync(xApiKey.Id, cancellationToken)
      ?? throw new ApiKeyNotFoundException(xApiKey.Id);

    apiKey.Authenticate(Convert.ToBase64String(xApiKey.Secret), _applicationContext.ActorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }
}
