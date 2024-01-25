using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityDomain()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddTransient<IRoleSettingsResolver, PortalRoleSettingsResolver>()
      .AddTransient<IUserSettingsResolver, PortalUserSettingsResolver>();
  }
}
