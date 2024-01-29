using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Settings;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
  {
    return services
      .AddFacades()
      .AddLogitarIdentityDomain()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton<IRoleSettingsResolver, PortalRoleSettingsResolver>()
      .AddSingleton<IUserSettingsResolver, PortalUserSettingsResolver>();
  }

  private static IServiceCollection AddFacades(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationService, ConfigurationFacade>()
      .AddTransient<ISessionService, SessionFacade>();
  }
}
