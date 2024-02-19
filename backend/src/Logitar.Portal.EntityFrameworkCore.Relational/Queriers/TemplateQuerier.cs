using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class TemplateQuerier : ITemplateQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<TemplateEntity> _templates;

  public TemplateQuerier(IActorService actorService, IApplicationContext applicationContext,
    PortalContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
    _templates = context.Templates;
  }

  public async Task<Template> ReadAsync(TemplateAggregate template, CancellationToken cancellationToken)
  {
    return await ReadAsync(template.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The template entity 'AggregateId={template.Id.Value}' could not be found.");
  }
  public async Task<Template?> ReadAsync(TemplateId id, CancellationToken cancellationToken)
    => await ReadAsync(id.AggregateId.ToGuid(), cancellationToken);
  public async Task<Template?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    TemplateEntity? template = await _templates.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (template == null || template.TenantId != _applicationContext.TenantId?.Value)
    {
      return null;
    }

    return await MapAsync(template, realm, cancellationToken);
  }

  public async Task<Template?> ReadAsync(string uniqueKey, CancellationToken cancellationToken)
  {
    string? tenantId = _applicationContext.TenantId?.Value;
    string uniqueKeyNormalized = uniqueKey.Trim().ToUpper();

    TemplateEntity? template = await _templates.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueKeyNormalized == uniqueKeyNormalized, cancellationToken);

    if (template == null)
    {
      return null;
    }

    return await MapAsync(template, _applicationContext.Realm, cancellationToken);
  }

  public async Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = _applicationContext.Realm;

    IQueryBuilder builder = _sqlHelper.QueryFrom(PortalDb.Templates.Table).SelectAll(PortalDb.Templates.Table)
      .ApplyRealmFilter(PortalDb.Templates.TenantId, realm)
      .ApplyIdFilter(PortalDb.Templates.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Templates.UniqueKey, PortalDb.Templates.DisplayName, PortalDb.Templates.Subject);

    if (!string.IsNullOrWhiteSpace(payload.ContentType))
    {
      builder.Where(PortalDb.Templates.ContentType, Operators.IsEqualTo(payload.ContentType.Trim().ToLower()));
    }

    IQueryable<TemplateEntity> query = _templates.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<TemplateEntity>? ordered = null;
    foreach (TemplateSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case TemplateSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case TemplateSort.Subject:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Subject) : query.OrderBy(x => x.Subject))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Subject) : ordered.ThenBy(x => x.Subject));
          break;
        case TemplateSort.UniqueKey:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueKey) : query.OrderBy(x => x.UniqueKey))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueKey) : ordered.ThenBy(x => x.UniqueKey));
          break;
        case TemplateSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    TemplateEntity[] templates = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Template> items = await MapAsync(templates, realm, cancellationToken);

    return new SearchResults<Template>(items, total);
  }

  private async Task<Template> MapAsync(TemplateEntity template, Realm? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([template], realm, cancellationToken)).Single();
  private async Task<IEnumerable<Template>> MapAsync(IEnumerable<TemplateEntity> templates, Realm? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = templates.SelectMany(template => template.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return templates.Select(template => mapper.ToTemplate(template, realm));
  }
}
