using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Infrastructure;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Infrastructure.Caching;
using Logitar.Portal.Infrastructure.Converters;
using Logitar.Portal.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityInfrastructure()
      .AddLogitarPortalApplication()
      .AddMemoryCache()
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton(InitializeCachingSettings)
      .AddTransient(InitializeEventSerializer);
  }

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection("Caching").Get<CachingSettings>() ?? new();
  }

  private static IEventSerializer InitializeEventSerializer(IServiceProvider serviceProvider)
  {
    EventSerializer eventSerializer = new(serviceProvider.GetLogitarIdentityJsonConverters());

    eventSerializer.RegisterConverter(new ConfigurationIdConverter());
    eventSerializer.RegisterConverter(new JwtSecretUnitConverter());
    eventSerializer.RegisterConverter(new RealmIdConverter());
    eventSerializer.RegisterConverter(new UniqueSlugConverter());

    return eventSerializer;
  }
}
