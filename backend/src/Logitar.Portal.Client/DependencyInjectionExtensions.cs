using Logitar.Portal.Client.Configurations;
using Logitar.Portal.Contracts.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Client;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalClient(this IServiceCollection services, IConfiguration configuration)
  {
    PortalSettings settings = configuration.GetSection("Portal").Get<PortalSettings>() ?? new();
    return services.AddLogitarPortalClient(settings);
  }
  public static IServiceCollection AddLogitarPortalClient(this IServiceCollection services, IPortalSettings settings)
  {
    return services
      .AddHttpClient()
      .AddSingleton(settings)
      .AddSingleton<IConfigurationClient, ConfigurationClient>();
  }
}
