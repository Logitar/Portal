using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Extensions;
using Logitar.Portal.Web;
using Logitar.Portal.Web.Extensions;
using Microsoft.Extensions.Primitives;

namespace Logitar.Portal;

internal class HttpContextParametersResolver : IContextParametersResolver
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public HttpContextParametersResolver(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public IContextParameters Resolve()
  {
    ContextParameters parameters = new()
    {
      ApiKey = Context.GetApiKey(),
      User = Context.GetUser(),
      Session = Context.GetSession()
    };

    if (Context.Request.Headers.TryGetValue(Headers.Realm, out StringValues realms))
    {
      IReadOnlyCollection<string> values = realms.ExtractValues();
      if (values.Count > 1)
      {
        throw new TooManyHeaderValuesException(Headers.Realm, expectedCount: 1, values.Count);
      }
      else if (values.Count == 1)
      {
        parameters.RealmOverride = values.Single();
      }
    }

    if (Context.Request.Headers.TryGetValue(Headers.User, out StringValues users))
    {
      IReadOnlyCollection<string> values = users.ExtractValues();
      if (values.Count > 1)
      {
        throw new TooManyHeaderValuesException(Headers.User, expectedCount: 1, values.Count);
      }
      else if (values.Count == 1)
      {
        parameters.ImpersonifiedUser = values.Single();
      }
    }

    return parameters;
  }
}
