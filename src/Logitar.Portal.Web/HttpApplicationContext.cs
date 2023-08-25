using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Extensions;

namespace Logitar.Portal.Web;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected HttpContext Context => _httpContextAccessor.HttpContext
    ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public ActorId ActorId
  {
    get
    {
      User? user = Context.GetUser();
      if (user != null)
      {
        return new ActorId(user.Id);
      }

      return new ActorId(Guid.Empty);
    }
  }
}
