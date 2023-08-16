using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Extensions;

namespace Logitar.Portal;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public HttpApplicationContext(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }

  protected HttpContext Context => _httpContextAccessor.HttpContext
    ?? throw new InvalidOperationException($"The {_httpContextAccessor.HttpContext} is required.");

  public ActorId ActorId { get; } = new(); // TODO(fpion): Authentication

  public ConfigurationAggregate Configuration
  {
    get => Context.GetConfiguration() ?? _cacheService.Configuration
      ?? throw new InvalidOperationException("The configuration could not be resolved.");
    set => Context.SetConfiguration(value);
  }
}
