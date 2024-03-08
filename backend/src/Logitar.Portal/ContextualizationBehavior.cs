using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Web.Extensions;
using MediatR;

namespace Logitar.Portal;

internal class ContextualizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  private readonly ICacheService _cacheService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public ContextualizationBehavior(ICacheService cacheService, IHttpContextAccessor httpContextAccessor)
  {
    _cacheService = cacheService;
    _httpContextAccessor = httpContextAccessor;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    HttpContext? httpContext = _httpContextAccessor.HttpContext;
    if (httpContext != null && request is Activity activity)
    {
      Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration has not been initialized yet.");
      ActivityContext context = new(configuration, httpContext.GetRealm(), httpContext.GetApiKey(), httpContext.GetUser(), httpContext.GetSession());
      activity.Contextualize(context);
    }

    return await next();
  }
}
