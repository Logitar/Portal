using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Infrastructure;
using Logitar.Portal.Application;
using Logitar.Portal.Infrastructure.Converters;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityInfrastructure()
      .AddLogitarPortalApplication()
      .AddTransient(InitializeEventSerializer);
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
