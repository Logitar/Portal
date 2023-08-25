using Logitar.Portal.Contracts.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Logitar.Portal.Client;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalClient(this IServiceCollection services, IConfiguration configuration)
  {
    return services.AddLogitarPortalClient(configuration.GetSection("Portal").Get<PortalSettings>());
  }

  public static IServiceCollection AddLogitarPortalClient(this IServiceCollection services, PortalSettings? settings = null)
  {
    if (settings != null)
    {
      services.AddSingleton(settings);
    }

    return services
      .AddHttpClient()
      .AddTransient<IConfigurationService, ConfigurationClient>();
  }
}
