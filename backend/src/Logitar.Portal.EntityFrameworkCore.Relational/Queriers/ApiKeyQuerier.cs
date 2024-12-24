using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
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
  private readonly IQueryHelper _queryHelper;

  public ApiKeyQuerier(IActorService actorService, IdentityContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _apiKeys = context.ApiKeys;
    _queryHelper = queryHelper;
  }

  public async Task<ApiKeyModel> ReadAsync(RealmModel? realm, ApiKey apiKey, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, apiKey.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The API key entity 'StreamId={apiKey.Id.Value}' could not be found.");
  }
  public async Task<ApiKeyModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, new ApiKeyId(realm?.GetTenantId(), new EntityId(id)), cancellationToken);
  }
  public async Task<ApiKeyModel?> ReadAsync(RealmModel? realm, ApiKeyId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    ApiKeyEntity? apiKey = await _apiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (apiKey == null || apiKey.TenantId != realm?.GetTenantId().Value)
    {
      return null;
    }

    return await MapAsync(apiKey, realm, cancellationToken);
  }

  public async Task<SearchResults<ApiKeyModel>> SearchAsync(RealmModel? realm, SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.QueryFrom(ApiKeys.Table).SelectAll(ApiKeys.Table)
      .ApplyRealmFilter(ApiKeys.TenantId, realm)
      .ApplyIdFilter(ApiKeys.StreamId, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, ApiKeys.DisplayName);

    if (payload.HasAuthenticated.HasValue)
    {
      NullOperator @operator = payload.HasAuthenticated.Value ? Operators.IsNotNull() : Operators.IsNull();
      builder.Where(ApiKeys.AuthenticatedOn, @operator);
    }
    if (payload.Status != null)
    {
      DateTime moment = payload.Status.Moment?.ToUniversalTime() ?? DateTime.UtcNow;
      builder.Where(ApiKeys.ExpiresOn, payload.Status.IsExpired ? Operators.IsLessThanOrEqualTo(moment) : Operators.IsGreaterThan(moment));
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

  private async Task<ApiKeyModel> MapAsync(ApiKeyEntity apiKey, RealmModel? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([apiKey], realm, cancellationToken)).Single();
  private async Task<IEnumerable<ApiKeyModel>> MapAsync(IEnumerable<ApiKeyEntity> apiKeys, RealmModel? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = apiKeys.SelectMany(apiKey => apiKey.GetActorIds());
    IEnumerable<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return apiKeys.Select(apiKey => mapper.ToApiKey(apiKey, realm));
  }
}
