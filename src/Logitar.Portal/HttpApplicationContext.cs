using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly ICacheService _cacheService;

  public HttpApplicationContext(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public Actor Actor { get; } = new(); // TODO(fpion): Authentication
  public ActorId ActorId => new(Actor.Id);

  public ConfigurationAggregate Configuration => _cacheService.Configuration
    ?? throw new InvalidOperationException("The configuration context item has not been set.");
}
