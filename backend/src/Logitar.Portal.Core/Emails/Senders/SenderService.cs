using FluentValidation;
using Logitar.Portal.Core.Emails.Senders.Models;
using Logitar.Portal.Core.Emails.Senders.Payloads;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Emails.Senders
{
  internal class SenderService : ISenderService
  {
    private readonly IMappingService _mappingService;
    private readonly ISenderQuerier _querier;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository<Sender> _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<Sender> _validator;

    public SenderService(
      IMappingService mappingService,
      ISenderQuerier querier,
      IRealmQuerier realmQuerier,
      IRepository<Sender> repository,
      IUserContext userContext,
      IValidator<Sender> validator
    )
    {
      _mappingService = mappingService;
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
        realm = await _realmQuerier.GetAsync(payload.Realm, readOnly: false, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      bool isDefault = await _querier.GetDefaultAsync(realm, readOnly: true, cancellationToken) == null;

      var sender = new Sender(payload, _userContext.Actor.Id, isDefault, realm);
      _validator.ValidateAndThrow(sender);

      await _repository.SaveAsync(sender, cancellationToken);

      return await _mappingService.MapAsync<SenderModel>(sender, cancellationToken);
    }

    public async Task<SenderModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      Sender sender = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(id);

      if (sender.IsDefault)
      {
        PagedList<Sender> senders = await _querier.GetPagedAsync(realm: sender.Realm?.Id.ToString(), readOnly: true, cancellationToken: cancellationToken);
        if (senders.Count > 1)
        {
          throw new CannotDeleteDefaultSenderException(id, _userContext.Actor.Id);
        }
      }

      sender.Delete(_userContext.Actor.Id);

      await _repository.SaveAsync(sender, cancellationToken);

      return await _mappingService.MapAsync<SenderModel>(sender, cancellationToken);
    }

    public async Task<SenderModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      Sender? sender = await _querier.GetAsync(id, readOnly: true, cancellationToken);
      if (sender == null)
      {
        return null;
      }

      return await _mappingService.MapAsync<SenderModel>(sender, cancellationToken);
    }

    public async Task<ListModel<SenderModel>> GetAsync(ProviderType? provider, string? realm, string? search,
      SenderSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<Sender> senders = await _querier.GetPagedAsync(provider, realm, search,
        sort, desc,
        index, count,
        readOnly: true, cancellationToken);

      return await _mappingService.MapAsync<Sender, SenderModel>(senders, cancellationToken);
    }

    public async Task<SenderModel?> GetDefaultAsync(string? realmId, CancellationToken cancellationToken)
    {
      Realm? realm = null;
      if (realmId != null)
      {
        realm = await _realmQuerier.GetAsync(realmId, readOnly: false, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(realmId);
      }

      Sender? sender = await _querier.GetDefaultAsync(realm, readOnly: true, cancellationToken);
      if (sender == null)
      {
        return null;
      }

      return await _mappingService.MapAsync<SenderModel>(sender, cancellationToken);
    }

    public async Task<SenderModel> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
      Sender sender = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(id);

      Sender @default = await _querier.GetDefaultAsync(sender.Realm, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(nameof(Sender.IsDefault));

      if (!sender.Equals(@default))
      {
        @default.SetDefault(_userContext.Actor.Id, isDefault: false);
        sender.SetDefault(_userContext.Actor.Id, isDefault: true);

        await _repository.SaveAsync(new[] { @default, sender }, cancellationToken);
      }

      return await _mappingService.MapAsync<SenderModel>(sender, cancellationToken);
    }

    public async Task<SenderModel> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Sender sender = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(id);

      sender.Update(payload, _userContext.Actor.Id);
      _validator.ValidateAndThrow(sender);

      await _repository.SaveAsync(sender, cancellationToken);

      return await _mappingService.MapAsync<SenderModel>(sender, cancellationToken);
    }
  }
}
