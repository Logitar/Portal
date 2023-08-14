using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Settings;
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
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddTransient<IConfigurationService, ConfigurationService>()
      .AddTransient<IConfigureOptions<PasswordSettings>, ConfigurePasswordSettings>()
      .AddTransient<IRequestPipeline, RequestPipeline>();
  }
}
