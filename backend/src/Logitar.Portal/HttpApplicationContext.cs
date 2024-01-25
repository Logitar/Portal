using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Extensions;

namespace Logitar.Portal;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext
    ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public ActorId ActorId { get; } = new(); // TODO(fpion): Actor Authentication

  public IRoleSettings RoleSettings => Context.GetRealm()?.GetRoleSettings()
    ?? _cacheService.GetConfiguration()?.GetRoleSettings()
    ?? throw new InvalidOperationException("The configuration has not been initialized yet.");
  public IUserSettings UserSettings => Context.GetRealm()?.GetUserSettings()
    ?? _cacheService.GetConfiguration()?.GetUserSettings()
    ?? throw new InvalidOperationException("The configuration has not been initialized yet.");

  public HttpApplicationContext(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }
}
