using Portal.Core;
using System.Net;

namespace Portal.Web
{
  internal class HttpUserContext : IUserContext
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext HttpContext => _httpContextAccessor.HttpContext
      ?? throw new InvalidOperationException($"The {nameof(HttpContext)} is required.");

    public Guid ActorId => HttpContext.GetUser()?.Id ?? HttpContext.GetApiKey()?.Id ?? Guid.Empty;
    public Guid Id => HttpContext.GetUser()?.Id ?? throw new ApiException(HttpStatusCode.Unauthorized, "The User context item is required.");
    public Guid SessionId => HttpContext.GetSession()?.Id ?? throw new ApiException(HttpStatusCode.Unauthorized, "The Session ID is required.");

    public string BaseUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
  }
}
