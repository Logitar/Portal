using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Settings;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    services.AddOptions<PasswordSettings>();

    return services
      .AddAutoMapper(assembly)
      .AddApplicationServices()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddTransient<IConfigureOptions<PasswordSettings>, ConfigurePasswordSettings>()
      .AddTransient<IRequestPipeline, RequestPipeline>();
  }

  private static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationService, ConfigurationService>()
      .AddTransient<IRealmService, RealmService>();
  }
}
