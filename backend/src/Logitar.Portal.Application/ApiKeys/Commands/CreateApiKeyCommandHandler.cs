using FluentValidation;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands
{
  internal class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, ApiKeyModel>
  {
    private const int ApiKeySecretLength = 32;

    private readonly IApiKeyQuerier _apiKeyQuerier;
    private readonly IValidator<ApiKey> _apiKeyValidator;
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public CreateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier,
      IValidator<ApiKey> apiKeyValidator,
      IPasswordService passwordService,
      IRepository repository,
      IUserContext userContext)
    {
      _apiKeyQuerier = apiKeyQuerier;
      _apiKeyValidator = apiKeyValidator;
      _passwordService = passwordService;
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<ApiKeyModel> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
      CreateApiKeyPayload payload = request.Payload;

      string keyHash = _passwordService.GenerateAndHash(ApiKeySecretLength, out byte[] secret);
      ApiKey apiKey = new(_userContext.ActorId, keyHash, payload.DisplayName, payload.Description, payload.ExpiresOn);
      _apiKeyValidator.ValidateAndThrow(apiKey);

      await _repository.SaveAsync(apiKey, cancellationToken);

      ApiKeyModel model = await _apiKeyQuerier.GetAsync(apiKey.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The API key 'Id={apiKey.Id}' could not be found.");

      model.XApiKey = new XApiKey(apiKey.Id, secret).ToString();

      return model;
    }
  }
}
