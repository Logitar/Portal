using AutoMapper;
using FluentValidation;
using Portal.Core.Emails.Senders;
using Portal.Core.Emails.Templates;
using Portal.Core.Realms.Models;
using Portal.Core.Realms.Payloads;

namespace Portal.Core.Realms
{
  internal class RealmService : IRealmService
  {
    private readonly IMapper _mapper;
    private readonly IRealmQuerier _querier;
    private readonly IRepository<Realm> _repository;
    private readonly ISenderQuerier _senderQuerier;
    private readonly ITemplateQuerier _templateQuerier;
    private readonly IUserContext _userContext;
    private readonly IValidator<Realm> _validator;

    public RealmService(
      IMapper mapper,
      IRealmQuerier querier,
      IRepository<Realm> repository,
      ISenderQuerier senderQuerier,
      ITemplateQuerier templateQuerier,
      IUserContext userContext,
      IValidator<Realm> validator
    )
    {
      _mapper = mapper;
      _querier = querier;
      _repository = repository;
      _senderQuerier = senderQuerier;
      _templateQuerier = templateQuerier;
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
      await UpdatePasswordRecoverySettingsAsync(realm, payload, cancellationToken);
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
      await UpdatePasswordRecoverySettingsAsync(realm, payload, cancellationToken);
      _validator.ValidateAndThrow(realm);

      await _repository.SaveAsync(realm, cancellationToken);

      return _mapper.Map<RealmModel>(realm);
    }

    private async Task UpdatePasswordRecoverySettingsAsync(Realm realm, SaveRealmPayload payload, CancellationToken cancellationToken)
    {
      if (payload.PasswordRecoverySenderId.HasValue)
      {
        Sender passwordRecoverySender = await _senderQuerier.GetAsync(payload.PasswordRecoverySenderId.Value, readOnly: false, cancellationToken)
          ?? throw new EntityNotFoundException<Sender>(payload.PasswordRecoverySenderId.Value, nameof(payload.PasswordRecoverySenderId));

        if (realm.Sid != passwordRecoverySender.RealmSid)
        {
          throw new SenderNotInRealmException(passwordRecoverySender, realm, nameof(payload.PasswordRecoverySenderId));
        }

        realm.PasswordRecoverySender = passwordRecoverySender;
      }
      else
      {
        realm.PasswordRecoverySender = null;
      }

      if (payload.PasswordRecoveryTemplateId.HasValue)
      {
        Template passwordRecoveryTemplate = await _templateQuerier.GetAsync(payload.PasswordRecoveryTemplateId.Value, readOnly: false, cancellationToken)
          ?? throw new EntityNotFoundException<Template>(payload.PasswordRecoveryTemplateId.Value, nameof(payload.PasswordRecoveryTemplateId));

        if (realm.Sid != passwordRecoveryTemplate.RealmSid)
        {
          throw new TemplateNotInRealmException(passwordRecoveryTemplate, realm, nameof(payload.PasswordRecoveryTemplateId));
        }

        realm.PasswordRecoveryTemplate = passwordRecoveryTemplate;
      }
      else
      {
        realm.PasswordRecoveryTemplate = null;
      }
    }
  }
}
