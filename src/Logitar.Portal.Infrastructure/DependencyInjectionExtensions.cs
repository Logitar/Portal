using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
  {
    // TODO(fpion): IPasswordService

    return services
      .AddLogitarPortalApplication()
      .AddSingleton<ICacheService, CacheService>()
      .AddScoped<IEventBus, EventBus>();
  }
}
