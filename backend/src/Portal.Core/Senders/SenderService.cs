using AutoMapper;
using FluentValidation;
using Portal.Core.Realms;
using Portal.Core.Senders.Models;
using Portal.Core.Senders.Payloads;

namespace Portal.Core.Senders
{
  internal class SenderService : ISenderService
  {
    private readonly IMapper _mapper;
    private readonly ISenderQuerier _querier;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository<Sender> _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<Sender> _validator;

    public SenderService(
      IMapper mapper,
      ISenderQuerier querier,
      IRealmQuerier realmQuerier,
      IRepository<Sender> repository,
      IUserContext userContext,
      IValidator<Sender> validator
    )
    {
      _mapper = mapper;
      _querier = querier;
      _realmQuerier = realmQuerier;
      _repository = repository;
      _userContext = userContext;
      _validator = validator;
    }

    public async Task<SenderModel> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = (Guid.TryParse(payload.Realm, out Guid guid)
          ? await _realmQuerier.GetAsync(guid, readOnly: false, cancellationToken)
          : await _realmQuerier.GetAsync(alias: payload.Realm, readOnly: false, cancellationToken)
        ) ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      bool isDefault = await _querier.GetDefaultAsync(realm, readOnly: true, cancellationToken) == null;

      var sender = new Sender(payload, _userContext.ActorId, isDefault, realm);
      _validator.ValidateAndThrow(sender);

      await _repository.SaveAsync(sender, cancellationToken);

      return _mapper.Map<SenderModel>(sender);
    }

    public async Task<SenderModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      Sender sender = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(id);

      if (sender.IsDefault)
      {
        PagedList<Sender> senders = await _querier.GetPagedAsync(realmId: sender.Realm?.Id, readOnly: true, cancellationToken: cancellationToken);
        if (senders.Count > 1)
        {
          throw new CannotDeleteDefaultSenderException(id, _userContext.ActorId);
        }
      }

      sender.Delete(_userContext.ActorId);

      await _repository.SaveAsync(sender, cancellationToken);

      return _mapper.Map<SenderModel>(sender);
    }

    public async Task<SenderModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      Sender? sender = await _querier.GetAsync(id, readOnly: true, cancellationToken);

      return _mapper.Map<SenderModel>(sender);
    }

    public async Task<ListModel<SenderModel>> GetAsync(ProviderType? provider, Guid? realmId, string? search,
      SenderSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<Sender> senders = await _querier.GetPagedAsync(provider, realmId, search,
        sort, desc,
        index, count,
        readOnly: true, cancellationToken);

      return ListModel<SenderModel>.From(senders, _mapper);
    }

    public async Task<SenderModel> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
      Sender sender = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(id);

      Sender @default = await _querier.GetDefaultAsync(sender.Realm, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(nameof(Sender.IsDefault));

      if (!sender.Equals(@default))
      {
        @default.SetDefault(_userContext.ActorId, isDefault: false);
        sender.SetDefault(_userContext.ActorId, isDefault: true);

        await _repository.SaveAsync(new[] { @default, sender }, cancellationToken);
      }

      return _mapper.Map<SenderModel>(sender);
    }

    public async Task<SenderModel> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Sender sender = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(id);

      sender.Update(payload, _userContext.ActorId);
      _validator.ValidateAndThrow(sender);

      await _repository.SaveAsync(sender, cancellationToken);

      return _mapper.Map<SenderModel>(sender);
    }
  }
}
