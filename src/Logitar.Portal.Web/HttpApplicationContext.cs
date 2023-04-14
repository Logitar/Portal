using Logitar.Portal.Core;
using Logitar.Portal.Web.Extensions;

namespace Logitar.Portal.Web;

public class HttpApplicationContext : IApplicationContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Guid? ActivityId
  {
    get => _httpContextAccessor.HttpContext?.GetActivityId();
    set => _httpContextAccessor.HttpContext?.SetActivityId(value);
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
}
