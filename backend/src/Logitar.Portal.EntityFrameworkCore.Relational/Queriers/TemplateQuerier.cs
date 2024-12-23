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
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<TemplateEntity> _templates;

  public TemplateQuerier(IActorService actorService, PortalContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
    _templates = context.Templates;
  }

  public async Task<Template> ReadAsync(RealmModel? realm, TemplateAggregate template, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, template.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The template entity 'AggregateId={template.Id.Value}' could not be found.");
  }
  public async Task<Template?> ReadAsync(RealmModel? realm, TemplateId id, CancellationToken cancellationToken)
    => await ReadAsync(realm, id.ToGuid(), cancellationToken);
  public async Task<Template?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    TemplateEntity? template = await _templates.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    if (template == null || template.TenantId != realm?.GetTenantId().Value)
    {
      return null;
    }

    return await MapAsync(template, realm, cancellationToken);
  }

  public async Task<Template?> ReadAsync(RealmModel? realm, string uniqueKey, CancellationToken cancellationToken)
  {
    string? tenantId = realm?.GetTenantId().Value;
    string uniqueKeyNormalized = uniqueKey.Trim().ToUpper();

    TemplateEntity? template = await _templates.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueKeyNormalized == uniqueKeyNormalized, cancellationToken);

    if (template == null)
    {
      return null;
    }

    return await MapAsync(template, realm, cancellationToken);
  }

  public async Task<SearchResults<Template>> SearchAsync(RealmModel? realm, SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
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

  private async Task<Template> MapAsync(TemplateEntity template, RealmModel? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([template], realm, cancellationToken)).Single();
  private async Task<IEnumerable<Template>> MapAsync(IEnumerable<TemplateEntity> templates, RealmModel? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = templates.SelectMany(template => template.GetActorIds());
    IEnumerable<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return templates.Select(template => mapper.ToTemplate(template, realm));
  }
}
