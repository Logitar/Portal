using Logitar.Portal.Application;
using Logitar.Portal.Domain.Actors;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Users;
using System.Net;

namespace Logitar.Portal.Web
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

    public Actor Actor
    {
      get
      {
        User? user = HttpContext.GetUser();
        if (user != null)
        {
          return new Actor(user);
        }

        ApiKey? apiKey = HttpContext.GetApiKey();
        if (apiKey != null)
        {
          return new Actor(apiKey);
        }

        return Actor.System;
      }
    }

    public Guid Id => HttpContext.GetUser()?.Id ?? throw new ApiException(HttpStatusCode.Unauthorized, "The User context item is required.");
    public Guid SessionId => HttpContext.GetSession()?.Id ?? throw new ApiException(HttpStatusCode.Unauthorized, "The Session ID is required.");

    public string BaseUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
  }
}
