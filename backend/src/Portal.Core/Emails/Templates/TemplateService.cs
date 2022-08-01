using AutoMapper;
using FluentValidation;
using Portal.Core.Emails.Templates.Models;
using Portal.Core.Emails.Templates.Payloads;
using Portal.Core.Realms;

namespace Portal.Core.Emails.Templates
{
  internal class TemplateService : ITemplateService
  {
    private readonly IMapper _mapper;
    private readonly ITemplateQuerier _querier;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository<Template> _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<Template> _validator;

    public TemplateService(
      IMapper mapper,
      ITemplateQuerier querier,
      IRealmQuerier realmQuerier,
      IRepository<Template> repository,
      IUserContext userContext,
      IValidator<Template> validator
    )
    {
      _mapper = mapper;
      _querier = querier;
      _realmQuerier = realmQuerier;
      _repository = repository;
      _userContext = userContext;
      _validator = validator;
    }

    public async Task<TemplateModel> CreateAsync(CreateTemplatePayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _realmQuerier.GetAsync(payload.Realm, readOnly: false, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      if (await _querier.GetByKeyAsync(payload.Key, realm, readOnly: true, cancellationToken) != null)
      {
        throw new KeyAlreadyUsedException(payload.Key, nameof(payload.Key));
      }

      var template = new Template(payload, _userContext.ActorId, realm);
      _validator.ValidateAndThrow(template);

      await _repository.SaveAsync(template, cancellationToken);

      return _mapper.Map<TemplateModel>(template);
    }

    public async Task<TemplateModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      Template template = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Template>(id);

      template.Delete(_userContext.ActorId);

      await _repository.SaveAsync(template, cancellationToken);

      return _mapper.Map<TemplateModel>(template);
    }

    public async Task<TemplateModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      Template? template = await _querier.GetAsync(id, readOnly: true, cancellationToken);

      return _mapper.Map<TemplateModel>(template);
    }

    public async Task<ListModel<TemplateModel>> GetAsync(string? realm, string? search,
      TemplateSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<Template> templates = await _querier.GetPagedAsync(realm, search,
        sort, desc,
        index, count,
        readOnly: true, cancellationToken);

      return ListModel<TemplateModel>.From(templates, _mapper);
    }

    public async Task<TemplateModel> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Template template = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Template>(id);

      template.Update(payload, _userContext.ActorId);
      _validator.ValidateAndThrow(template);

      await _repository.SaveAsync(template, cancellationToken);

      return _mapper.Map<TemplateModel>(template);
    }
  }
}
