using FluentValidation;
using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Actors;
using Logitar.Portal.Core.ApiKeys.Models;
using Logitar.Portal.Core.ApiKeys.Payloads;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.ApiKeys
{
  internal class ApiKeyService : IApiKeyService
  {
    private const int ApiKeySecretLength = 32;

    private readonly IActorService _actorService;
    private readonly IMappingService _mappingService;
    private readonly IPasswordService _passwordService;
    private readonly IApiKeyQuerier _querier;
    private readonly IRepository<ApiKey> _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<ApiKey> _validator;

    public ApiKeyService(
      IActorService actorService,
      IMappingService mappingService,
      IPasswordService passwordService,
      IApiKeyQuerier querier,
      IRepository<ApiKey> repository,
      IUserContext userContext,
      IValidator<ApiKey> validator
    )
    {
      _actorService = actorService;
      _mappingService = mappingService;
      _passwordService = passwordService;
      _querier = querier;
      _repository = repository;
      _userContext = userContext;
      _validator = validator;
    }

    public async Task<ApiKeyModel> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      string keyHash = _passwordService.GenerateAndHash(ApiKeySecretLength, out byte[] secretBytes);

      var apiKey = new ApiKey(keyHash, payload, _userContext.Actor.Id);
      _validator.ValidateAndThrow(apiKey);

      await _repository.SaveAsync(apiKey, cancellationToken);

      var model = await _mappingService.MapAsync<ApiKeyModel>(apiKey, cancellationToken);
      model.XApiKey = new SecureToken(apiKey.Id, secretBytes).ToString();

      return model;
    }

    public async Task<ApiKeyModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      ApiKey apiKey = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<ApiKey>(id);

      apiKey.Delete(_userContext.Actor.Id);

      await _repository.SaveAsync(apiKey, cancellationToken);
      await _actorService.SaveAsync(apiKey, cancellationToken);

      return await _mappingService.MapAsync<ApiKeyModel>(apiKey, cancellationToken);
    }

    public async Task<ApiKeyModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      ApiKey? apiKey = await _querier.GetAsync(id, readOnly: false, cancellationToken);
      if (apiKey == null)
      {
        return null;
      }

      return await _mappingService.MapAsync<ApiKeyModel>(apiKey, cancellationToken);
    }

    public async Task<ListModel<ApiKeyModel>> GetAsync(bool? isExpired, string? search,
      ApiKeySort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<ApiKey> apiKeys = await _querier.GetPagedAsync(isExpired, search,
        sort, desc,
        index, count,
        readOnly: true, cancellationToken);

      return await _mappingService.MapAsync<ApiKey, ApiKeyModel>(apiKeys, cancellationToken);
    }

    public async Task<ApiKeyModel> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      ApiKey apiKey = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<ApiKey>(id);

      apiKey.Update(payload, _userContext.Actor.Id);
      _validator.ValidateAndThrow(apiKey);

      await _repository.SaveAsync(apiKey, cancellationToken);
      await _actorService.SaveAsync(apiKey, cancellationToken);

      return await _mappingService.MapAsync<ApiKeyModel>(apiKey, cancellationToken);
    }
  }
}
