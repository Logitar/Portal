using AutoMapper;
using FluentValidation;
using Portal.Core.Realms.Models;
using Portal.Core.Realms.Payloads;

namespace Portal.Core.Realms
{
  internal class RealmService : IRealmService
  {
    private readonly IMapper _mapper;
    private readonly IRealmQuerier _querier;
    private readonly IRepository<Realm> _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<Realm> _validator;

    public RealmService(
      IMapper mapper,
      IRealmQuerier querier,
      IRepository<Realm> repository,
      IUserContext userContext,
      IValidator<Realm> validator
    )
    {
      _mapper = mapper;
      _querier = querier;
      _repository = repository;
      _userContext = userContext;
      _validator = validator;
    }

    public async Task<RealmModel> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      if (await _querier.GetAsync(payload.Alias, readOnly: true, cancellationToken) != null)
      {
        throw new AliasAlreadyUsedException(payload.Alias, nameof(payload.Alias));
      }

      var realm = new Realm(payload, _userContext.ActorId);
      _validator.ValidateAndThrow(realm);

      await _repository.SaveAsync(realm, cancellationToken);

      return _mapper.Map<RealmModel>(realm);
    }

    public async Task<RealmModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      Realm realm = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Realm>(id);

      realm.Delete(_userContext.ActorId);

      await _repository.SaveAsync(realm, cancellationToken);

      return _mapper.Map<RealmModel>(realm);
    }

    public async Task<RealmModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      Realm? realm = Guid.TryParse(id, out Guid guid)
        ? await _querier.GetAsync(guid, readOnly: true, cancellationToken)
        : await _querier.GetAsync(alias: id, readOnly: true, cancellationToken);

      return _mapper.Map<RealmModel>(realm);
    }

    public async Task<ListModel<RealmModel>> GetAsync(string? search,
      RealmSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<Realm> realms = await _querier.GetPagedAsync(search,
        sort, desc,
        index, count,
        readOnly: true, cancellationToken);

      return ListModel<RealmModel>.From(realms, _mapper);
    }

    public async Task<RealmModel> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Realm realm = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Realm>(id);

      realm.Update(payload, _userContext.ActorId);
      _validator.ValidateAndThrow(realm);

      await _repository.SaveAsync(realm, cancellationToken);

      return _mapper.Map<RealmModel>(realm);
    }
  }
}
