using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Web.Extensions;

namespace Logitar.Portal.Web;

public class HttpApplicationContext : IApplicationContext
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public HttpApplicationContext(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }

  public Guid? ActivityId
  {
    get => _httpContextAccessor.HttpContext?.GetActivityId();
    set => _httpContextAccessor.HttpContext?.SetActivityId(value);
  }
  public AggregateId ActorId
  {
    get
    {
      Guid id = Guid.Empty;

      if (_httpContextAccessor.HttpContext != null)
      {
        User? user = _httpContextAccessor.HttpContext.GetUser();
        if (user != null)
        {
          id = user.Id;
        }
      }

      return new AggregateId(id);
    }
  }

  public Uri? BaseUrl
  {
    get
    {
      if (_httpContextAccessor.HttpContext == null)
      {
        return null;
      }

      HttpRequest request = _httpContextAccessor.HttpContext.Request;

      return new Uri($"{request.Scheme}://{request.Host}");
    }
  }

  public ConfigurationAggregate Configuration => _cacheService.Configuration
    ?? throw new InvalidOperationException("The configuration could not be found in the cache.");
}
