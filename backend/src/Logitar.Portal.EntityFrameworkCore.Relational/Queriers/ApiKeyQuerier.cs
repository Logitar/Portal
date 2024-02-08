using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class ApiKeyQuerier : IApiKeyQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<ApiKeyEntity> _apiKeys;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ApiKeyQuerier(IActorService actorService, IApplicationContext applicationContext,
    IdentityContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _apiKeys = context.ApiKeys;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<ApiKey> ReadAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken)
  {
    return await ReadAsync(apiKey.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The API key entity 'AggregateId={apiKey.Id.Value}' could not be found.");
  }
  public async Task<ApiKey?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken)
    => await ReadAsync(id.AggregateId.ToGuid(), cancellationToken);
  public async Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ApiKeyEntity? apiKey = await _apiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (apiKey == null || apiKey.TenantId != _applicationContext.TenantId?.Value)
    {
      return null;
    }

    return await MapAsync(apiKey, realm, cancellationToken);
  }

  private async Task<ApiKey> MapAsync(ApiKeyEntity apiKey, Realm? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([apiKey], realm, cancellationToken)).Single();
  private async Task<IEnumerable<ApiKey>> MapAsync(IEnumerable<ApiKeyEntity> apiKeys, Realm? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = apiKeys.SelectMany(apiKey => apiKey.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return apiKeys.Select(apiKey => mapper.ToApiKey(apiKey, realm));
  }
}
