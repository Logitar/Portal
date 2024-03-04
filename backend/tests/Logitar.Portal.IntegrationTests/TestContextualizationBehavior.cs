using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Security.Cryptography;
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
      //Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration has not been initialized yet.");
      DateTime now = DateTime.UtcNow;
      Configuration configuration = _cacheService.Configuration ?? new()
      {
        Version = 1,
        CreatedOn = now,
        UpdatedOn = now,
        DefaultLocale = new Locale("en"),
        Secret = RandomStringGenerator.GetString(),
        RequireUniqueEmail = true
      }; // TODO(fpion): hack for InitializeConfigurationCommand that will not be an ApplicationRequest eventually
      ApplicationContext context = _context.ToApplicationContext(configuration);
      applicationRequest.Contextualize(context);
    }

    return await next();
  }
}
