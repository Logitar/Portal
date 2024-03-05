using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal;

internal class TestContextualizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  private readonly ICacheService _cacheService;
  private readonly TestContext _context;

  public TestContextualizationBehavior(ICacheService cacheService, TestContext context)
  {
    _cacheService = cacheService;
    _context = context;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (request is ApplicationRequest applicationRequest)
    {
      Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration should be in the cache.");
      ApplicationContext context = _context.ToApplicationContext(configuration);
      applicationRequest.Contextualize(context);
    }

    return await next();
  }
}
