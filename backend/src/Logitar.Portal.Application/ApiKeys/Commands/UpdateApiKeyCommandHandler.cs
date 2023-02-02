using FluentValidation;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands
{
  internal class UpdateApiKeyCommandHandler : IRequestHandler<UpdateApiKeyCommand, ApiKeyModel>
  {
    private readonly IApiKeyQuerier _apiKeyQuerier;
    private readonly IValidator<ApiKey> _apiKeyValidator;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public UpdateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier,
      IValidator<ApiKey> apiKeyValidator,
      IRepository repository,
      IUserContext userContext)
    {
      _apiKeyQuerier = apiKeyQuerier;
      _apiKeyValidator = apiKeyValidator;
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<ApiKeyModel> Handle(UpdateApiKeyCommand request, CancellationToken cancellationToken)
    {
      UpdateApiKeyPayload payload = request.Payload;

      ApiKey apiKey = await _repository.LoadAsync<ApiKey>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<ApiKey>(request.Id);
      apiKey.Update(_userContext.ActorId, payload.DisplayName, payload.Description);
      _apiKeyValidator.ValidateAndThrow(apiKey);

      await _repository.SaveAsync(apiKey, cancellationToken);

      return await _apiKeyQuerier.GetAsync(apiKey.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The API key 'Id={apiKey.Id}' could not be found.");
    }
  }
}
