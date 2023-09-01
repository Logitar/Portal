using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Realms;
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

  public ApiKeyQuerier(IActorService actorService, PortalContext context, IRealmQuerier realmQuerier)
  {
    _actorService = actorService;
    _apiKeys = context.ApiKeys;
    _realmQuerier = realmQuerier;
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

  private async Task<IEnumerable<ApiKey>> MapAsync(Realm? realm = null, CancellationToken cancellationToken = default, params ApiKeyEntity[] apiKeys)
  {
    IEnumerable<ActorId> actorIds = apiKeys.SelectMany(user => user.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return apiKeys.Select(apiKey => mapper.ToApiKey(apiKey, realm));
  }
}
