using Logitar.Portal.Application;
using Logitar.Portal.Web.Extensions;

namespace Logitar.Portal;

internal class BaseUrl : IBaseUrl
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public string Value => Context.GetBaseUri().ToString();

  public BaseUrl(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }
}
