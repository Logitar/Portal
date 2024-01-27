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
  private readonly DbSet<ApiKeyEntity> _apiKeys;
  private readonly IApplicationContext _applicationContext;

  public ApiKeyQuerier(IActorService actorService, IApplicationContext applicationContext, IdentityContext context)
  {
    _actorService = actorService;
    _apiKeys = context.ApiKeys;
    _applicationContext = applicationContext;
  }

  public async Task<ApiKey> ReadAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken)
  {
    return await ReadAsync(apiKey.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The API key entity 'AggregateId={apiKey.Id.Value}' could not be found.");
  }
  public async Task<ApiKey?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken)
    => await ReadAsync(id.Value, cancellationToken);
  public async Task<ApiKey?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    string aggregateId = id.Trim();

    ApiKeyEntity? apiKey = await _apiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (apiKey == null || apiKey.TenantId != realm?.Id)
    {
      return null;
    }

    return await MapAsync(apiKey, realm, cancellationToken);
  }

  private async Task<ApiKey> MapAsync(ApiKeyEntity apiKey, Realm? realm, CancellationToken cancellationToken)
  => (await MapAsync([apiKey], realm, cancellationToken)).Single();
  private async Task<IEnumerable<ApiKey>> MapAsync(IEnumerable<ApiKeyEntity> apiKeys, Realm? realm, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = apiKeys.SelectMany(apiKey => apiKey.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return apiKeys.Select(apiKey => mapper.ToApiKey(apiKey, realm));
  }
}
