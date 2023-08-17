using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal;

internal class TestApplicationContext : IApplicationContext
{
  private ConfigurationAggregate? _configuration = null;

  private readonly ICacheService _cacheService;

  public TestApplicationContext(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public ActorId ActorId { get; } = new(); // TODO(fpion): Authentication

  public ConfigurationAggregate Configuration
  {
    get => _configuration ?? _cacheService.Configuration
      ?? throw new InvalidOperationException("The configuration could not be resolved.");
    set => _configuration = value;
  }
  public RealmAggregate? Realm { get; set; }
}
