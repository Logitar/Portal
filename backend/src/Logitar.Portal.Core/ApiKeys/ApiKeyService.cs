using AutoMapper;
using FluentValidation;
using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.ApiKeys.Models;
using Logitar.Portal.Core.ApiKeys.Payloads;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.ApiKeys
{
  internal class ApiKeyService : IApiKeyService
  {
    private const int ApiKeySecretLength = 32;

    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;
    private readonly IApiKeyQuerier _querier;
    private readonly IRepository<ApiKey> _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<ApiKey> _validator;

    public ApiKeyService(
      IMapper mapper,
      IPasswordService passwordService,
      IApiKeyQuerier querier,
      IRepository<ApiKey> repository,
      IUserContext userContext,
      IValidator<ApiKey> validator
    )
    {
      _mapper = mapper;
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

      var apiKey = new ApiKey(keyHash, payload, _userContext.ActorId);
      _validator.ValidateAndThrow(apiKey);

      await _repository.SaveAsync(apiKey, cancellationToken);

      var model = _mapper.Map<ApiKeyModel>(apiKey);
      model.XApiKey = new SecureToken(apiKey.Id, secretBytes).ToString();

      return model;
    }

    public async Task<ApiKeyModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      ApiKey apiKey = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<ApiKey>(id);

      apiKey.Delete(_userContext.ActorId);

      await _repository.SaveAsync(apiKey, cancellationToken);

      return _mapper.Map<ApiKeyModel>(apiKey);
    }

    public async Task<ApiKeyModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      ApiKey? apiKey = await _querier.GetAsync(id, readOnly: false, cancellationToken);

      return _mapper.Map<ApiKeyModel>(apiKey);
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

      return ListModel<ApiKeyModel>.From(apiKeys, _mapper);
    }

    public async Task<ApiKeyModel> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      ApiKey apiKey = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<ApiKey>(id);

      apiKey.Update(payload, _userContext.ActorId);
      _validator.ValidateAndThrow(apiKey);

      await _repository.SaveAsync(apiKey, cancellationToken);

      return _mapper.Map<ApiKeyModel>(apiKey);
    }
  }
}
