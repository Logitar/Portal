using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarPortalApplication()
      .AddSingleton<ICacheService, CacheService>();
  }
}
