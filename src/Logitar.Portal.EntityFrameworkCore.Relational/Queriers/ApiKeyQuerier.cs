using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class ApiKeyQuerier : IApiKeyQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ApiKeyEntity> _apiKeys;
  private readonly IRealmQuerier _realmQuerier;
  private readonly ISqlHelper _sqlHelper;

  public ApiKeyQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _apiKeys = context.ApiKeys;
    _realmQuerier = realmQuerier;
    _sqlHelper = sqlHelper;
  }

  public async Task<ApiKey> ReadAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken)
  {
    return await ReadAsync(apiKey.Id, cancellationToken)
      ?? throw new EntityNotFoundException<ApiKeyEntity>(apiKey.Id);
  }
  public async Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new AggregateId(id), cancellationToken);
  }
  private async Task<ApiKey?> ReadAsync(AggregateId id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Value;

    ApiKeyEntity? apiKey = await _apiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (apiKey == null)
    {
      return null;
    }

    Realm? realm = null;
    if (apiKey.TenantId != null)
    {
      AggregateId realmId = new(apiKey.TenantId);
      realm = await _realmQuerier.ReadAsync(realmId, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(realmId);
    }

    return (await MapAsync(realm, cancellationToken, apiKey)).Single();
  }

  public async Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    Realm? realm = null;
    string? tenantId = null;
    if (!string.IsNullOrWhiteSpace(payload.Realm))
    {
      realm = await _realmQuerier.FindAsync(payload.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<RealmEntity>(payload.Realm);
      tenantId = new AggregateId(realm.Id).Value;
    }

    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.ApiKeys.Table)
      .ApplyIdInFilter(Db.ApiKeys.AggregateId, payload.IdIn)
      .Where(Db.ApiKeys.TenantId, tenantId == null ? Operators.IsNull() : Operators.IsEqualTo(tenantId))
      .SelectAll(Db.ApiKeys.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.ApiKeys.Title);

    if (payload.Status != null)
    {
      DateTime moment = payload.Status.Moment ?? DateTime.UtcNow;
      builder = payload.Status.IsExpired
        ? builder.Where(Db.ApiKeys.ExpiresOn, Operators.IsLessThanOrEqualTo(moment))
        : builder.WhereOr(
          new OperatorCondition(Db.ApiKeys.ExpiresOn, Operators.IsNull()),
          new OperatorCondition(Db.ApiKeys.ExpiresOn, Operators.IsGreaterThan(moment))
        );
    }

    IQueryable<ApiKeyEntity> query = _apiKeys.FromQuery(builder.Build())
      .AsNoTracking()
      .Include(x => x.Roles);
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ApiKeyEntity>? ordered = null;
    foreach (ApiKeySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ApiKeySort.AuthenticatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.AuthenticatedOn) : query.OrderBy(x => x.AuthenticatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.AuthenticatedOn) : ordered.ThenBy(x => x.AuthenticatedOn));
          break;
        case ApiKeySort.ExpiresOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.ExpiresOn) : query.OrderBy(x => x.ExpiresOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.ExpiresOn) : ordered.ThenBy(x => x.ExpiresOn));
          break;
        case ApiKeySort.Title:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Title) : ordered.ThenBy(x => x.Title));
          break;
        case ApiKeySort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ApiKeyEntity[] apiKeys = await query.ToArrayAsync(cancellationToken);
    IEnumerable<ApiKey> results = await MapAsync(realm, cancellationToken, apiKeys);

    return new SearchResults<ApiKey>(results, total);
  }

  private async Task<IEnumerable<ApiKey>> MapAsync(Realm? realm = null, CancellationToken cancellationToken = default, params ApiKeyEntity[] apiKeys)
  {
    IEnumerable<ActorId> actorIds = apiKeys.SelectMany(user => user.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return apiKeys.Select(apiKey => mapper.ToApiKey(apiKey, realm));
  }
}
