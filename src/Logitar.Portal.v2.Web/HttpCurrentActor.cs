using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core;
using Logitar.Portal.v2.Web.Extensions;

namespace Logitar.Portal.v2.Web;

internal class HttpCurrentActor : ICurrentActor
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public HttpCurrentActor(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public AggregateId Id
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

        // TODO(fpion): ApiKey
      }

      return new AggregateId(id);
    }
  }
}
