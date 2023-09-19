using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class TemplateQuerier : ITemplateQuerier
{
  private readonly IActorService _actorService;
  private readonly IRealmQuerier _realmQuerier;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<TemplateEntity> _templates;

  public TemplateQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _realmQuerier = realmQuerier;
    _sqlHelper = sqlHelper;
    _templates = context.Templates;
  }

  public async Task<Template> ReadAsync(TemplateAggregate template, CancellationToken cancellationToken)
  {
    return await ReadAsync(template.Id, cancellationToken)
      ?? throw new EntityNotFoundException<TemplateEntity>(template.Id);
  }
  public async Task<Template?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new AggregateId(id), cancellationToken);
  }
  private async Task<Template?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Value;

    TemplateEntity? template = await _templates.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (template == null)
    {
      return null;
    }

    Realm? realm = null;
    if (template.TenantId != null)
    {
      AggregateId realmId = new(template.TenantId);
      realm = await _realmQuerier.ReadAsync(realmId, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmId);
    }

    return (await MapAsync(realm, cancellationToken, template)).Single();
  }

  public async Task<Template?> ReadAsync(string? realmIdOrUniqueSlug, string uniqueName, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    if (!string.IsNullOrWhiteSpace(realmIdOrUniqueSlug))
    {
      realm = await _realmQuerier.FindAsync(realmIdOrUniqueSlug, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmIdOrUniqueSlug);
    }

    string? tenantId = realm == null ? null : new AggregateId(realm.Id).Value;
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    TemplateEntity? template = await _templates.AsNoTracking()
      .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    if (template == null)
    {
      return null;
    }

    return (await MapAsync(realm, cancellationToken, template)).Single();
  }

  public async Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    string? tenantId = null;
    if (!string.IsNullOrWhiteSpace(payload.Realm))
    {
      realm = await _realmQuerier.FindAsync(payload.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(payload.Realm);
      tenantId = new AggregateId(realm.Id).Value;
    }

    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Templates.Table)
      .ApplyIdInFilter(Db.Templates.AggregateId, payload.IdIn)
      .Where(Db.Templates.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.Templates.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Templates.UniqueName, Db.Templates.DisplayName, Db.Templates.Subject);

    if (!string.IsNullOrWhiteSpace(payload.ContentType))
    {
      string contentType = payload.ContentType.Trim().ToLower();
      builder = builder.Where(Db.Templates.ContentType, Operators.IsEqualTo(contentType));
    }

    IQueryable<TemplateEntity> query = _templates.FromQuery(builder.Build())
      .AsNoTracking();
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
        case TemplateSort.UniqueName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
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
    IEnumerable<Template> results = await MapAsync(realm, cancellationToken, templates);

    return new SearchResults<Template>(results, total);
  }

  private async Task<IEnumerable<Template>> MapAsync(Realm? realm = null, CancellationToken cancellationToken = default, params TemplateEntity[] templates)
  {
    IEnumerable<ActorId> actorIds = templates.SelectMany(template => template.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return templates.Select(template => mapper.ToTemplate(template, realm));
  }
}
