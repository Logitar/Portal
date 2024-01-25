using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Extensions;

namespace Logitar.Portal;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly DefaultHttpContext _defaultContext = new();
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? _defaultContext;

  public ActorId ActorId { get; } = new(); // TODO(fpion): Actor Authentication

  public Realm Realm
  {
    get => Context.GetRealm() ?? throw new InvalidOperationException("The Realm context item has not been set.");
    set
    {
      if (Context.GetRealm() != null)
      {
        throw new InvalidOperationException("The Realm context item has already been set.");
      }
      Context.SetRealm(value);
    }
  }

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }
}
