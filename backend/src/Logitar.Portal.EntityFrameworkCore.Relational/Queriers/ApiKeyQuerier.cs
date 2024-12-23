using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class ApiKeyQuerier : IApiKeyQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ApiKeyEntity> _apiKeys;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ApiKeyQuerier(IActorService actorService, IdentityContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _apiKeys = context.ApiKeys;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<ApiKeyModel> ReadAsync(Realm? realm, ApiKey apiKey, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, apiKey.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The API key entity 'AggregateId={apiKey.Id.Value}' could not be found.");
  }
  public async Task<ApiKeyModel?> ReadAsync(Realm? realm, ApiKeyId id, CancellationToken cancellationToken)
    => await ReadAsync(realm, id.ToGuid(), cancellationToken);
  public async Task<ApiKeyModel?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ApiKeyEntity? apiKey = await _apiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    if (apiKey == null || apiKey.TenantId != realm?.GetTenantId()?.Value)
    {
      return null;
    }

    return await MapAsync(apiKey, realm, cancellationToken);
  }

  public async Task<SearchResults<ApiKeyModel>> SearchAsync(Realm? realm, SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(IdentityDb.ApiKeys.Table).SelectAll(IdentityDb.ApiKeys.Table)
      .ApplyRealmFilter(IdentityDb.ApiKeys.TenantId, realm)
      .ApplyIdFilter(IdentityDb.ApiKeys.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, IdentityDb.ApiKeys.DisplayName);

    if (payload.HasAuthenticated.HasValue)
    {
      NullOperator @operator = payload.HasAuthenticated.Value ? Operators.IsNotNull() : Operators.IsNull();
      builder.Where(IdentityDb.ApiKeys.AuthenticatedOn, @operator);
    }
    if (payload.Status != null)
    {
      DateTime moment = payload.Status.Moment?.ToUniversalTime() ?? DateTime.UtcNow;
      builder.Where(IdentityDb.ApiKeys.ExpiresOn, payload.Status.IsExpired ? Operators.IsLessThanOrEqualTo(moment) : Operators.IsGreaterThan(moment));
    }

    IQueryable<ApiKeyEntity> query = _apiKeys.FromQuery(builder).AsNoTracking()
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
        case ApiKeySort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case ApiKeySort.ExpiresOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.ExpiresOn) : query.OrderBy(x => x.ExpiresOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.ExpiresOn) : ordered.ThenBy(x => x.ExpiresOn));
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
    IEnumerable<ApiKeyModel> items = await MapAsync(apiKeys, realm, cancellationToken);

    return new SearchResults<ApiKeyModel>(items, total);
  }

  private async Task<ApiKeyModel> MapAsync(ApiKeyEntity apiKey, Realm? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([apiKey], realm, cancellationToken)).Single();
  private async Task<IEnumerable<ApiKeyModel>> MapAsync(IEnumerable<ApiKeyEntity> apiKeys, Realm? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = apiKeys.SelectMany(apiKey => apiKey.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return apiKeys.Select(apiKey => mapper.ToApiKey(apiKey, realm));
  }
}
