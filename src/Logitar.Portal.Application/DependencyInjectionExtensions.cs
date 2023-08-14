using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddTransient<ConfigurePasswordSettings>()
      .AddTransient<IConfigurationService, ConfigurationService>()
      .AddTransient<IRequestPipeline, RequestPipeline>();
  }
}
