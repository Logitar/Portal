using FluentValidation;
using Logitar.Portal.Core.Emails.Templates.Models;
using Logitar.Portal.Core.Emails.Templates.Payloads;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Emails.Templates
{
  internal class TemplateService : ITemplateService
  {
    private readonly IMappingService _mappingService;
    private readonly ITemplateQuerier _querier;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository<Template> _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<Template> _validator;

    public TemplateService(
      IMappingService mappingService,
      ITemplateQuerier querier,
      IRealmQuerier realmQuerier,
      IRepository<Template> repository,
      IUserContext userContext,
      IValidator<Template> validator
    )
    {
      _mappingService = mappingService;
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

      var template = new Template(payload, _userContext.Actor.Id, realm);
      _validator.ValidateAndThrow(template);

      await _repository.SaveAsync(template, cancellationToken);

      return await _mappingService.MapAsync<TemplateModel>(template, cancellationToken);
    }

    public async Task<TemplateModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      Template template = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Template>(id);

      template.Delete(_userContext.Actor.Id);

      await _repository.SaveAsync(template, cancellationToken);

      return await _mappingService.MapAsync<TemplateModel>(template, cancellationToken);
    }

    public async Task<TemplateModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      Template? template = await _querier.GetAsync(id, readOnly: true, cancellationToken);
      if (template == null)
      {
        return null;
      }

      return await _mappingService.MapAsync<TemplateModel>(template, cancellationToken);
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

      return await _mappingService.MapAsync<Template, TemplateModel>(templates, cancellationToken);
    }

    public async Task<TemplateModel> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Template template = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Template>(id);

      template.Update(payload, _userContext.Actor.Id);
      _validator.ValidateAndThrow(template);

      await _repository.SaveAsync(template, cancellationToken);

      return await _mappingService.MapAsync<TemplateModel>(template, cancellationToken);
    }
  }
}
