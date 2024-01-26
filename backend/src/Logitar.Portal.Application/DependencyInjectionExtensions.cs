using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Settings;
using Logitar.Portal.Contracts.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
  {
    return services
      .AddApplicationFacades()
      .AddLogitarIdentityDomain()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddTransient<IRoleSettingsResolver, PortalRoleSettingsResolver>()
      .AddTransient<IUserSettingsResolver, PortalUserSettingsResolver>();
  }

  private static IServiceCollection AddApplicationFacades(this IServiceCollection services)
  {
    return services.AddTransient<IConfigurationService, ConfigurationFacade>();
  }
}
