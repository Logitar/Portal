using FluentValidation;
using Logitar.Portal.Core.Dictionaries.Models;
using Logitar.Portal.Core.Dictionaries.Payloads;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Dictionaries
{
  internal class DictionaryService : IDictionaryService
  {
    private readonly IMappingService _mappingService;
    private readonly IDictionaryQuerier _querier;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository<Dictionary> _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<Dictionary> _validator;

    public DictionaryService(
      IMappingService mappingService,
      IDictionaryQuerier querier,
      IRealmQuerier realmQuerier,
      IRepository<Dictionary> repository,
      IUserContext userContext,
      IValidator<Dictionary> validator
    )
    {
      _mappingService = mappingService;
      _querier = querier;
      _realmQuerier = realmQuerier;
      _repository = repository;
      _userContext = userContext;
      _validator = validator;
    }

    public async Task<DictionaryModel> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _realmQuerier.GetAsync(payload.Realm, readOnly: false, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      var dictionary = new Dictionary(payload, _userContext.Actor.Id, realm);
      _validator.ValidateAndThrow(dictionary);

      await _repository.SaveAsync(dictionary, cancellationToken);

      return await _mappingService.MapAsync<DictionaryModel>(dictionary, cancellationToken);
    }

    public async Task<DictionaryModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      Dictionary dictionary = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Dictionary>(id);

      dictionary.Delete(_userContext.Actor.Id);

      await _repository.SaveAsync(dictionary, cancellationToken);

      return await _mappingService.MapAsync<DictionaryModel>(dictionary, cancellationToken);
    }

    public async Task<DictionaryModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      Dictionary? dictionary = await _querier.GetAsync(id, readOnly: true, cancellationToken);
      if (dictionary == null)
      {
        return null;
      }

      return await _mappingService.MapAsync<DictionaryModel>(dictionary, cancellationToken);
    }

    public async Task<ListModel<DictionaryModel>> GetAsync(string? locale, string? search,
      DictionarySort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<Dictionary> dictionaries = await _querier.GetPagedAsync(locale, search,
        sort, desc,
        index, count,
        readOnly: true, cancellationToken);

      return await _mappingService.MapAsync<Dictionary, DictionaryModel>(dictionaries, cancellationToken);
    }

    public async Task<DictionaryModel> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Dictionary dictionary = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Dictionary>(id);

      dictionary.Update(payload, _userContext.Actor.Id);
      _validator.ValidateAndThrow(dictionary);

      await _repository.SaveAsync(dictionary, cancellationToken);

      return await _mappingService.MapAsync<DictionaryModel>(dictionary, cancellationToken);
    }
  }
}
