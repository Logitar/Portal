using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Contracts.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddApplicationServices()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddTransient<IRequestPipeline, RequestPipeline>();
  }

  private static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationService, ConfigurationService>();
  }
}
