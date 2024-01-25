using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Application.Seeding;
using Logitar.Portal.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityDomain()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton(serviceProvider =>
      {
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetSection("Seeding").Get<SeedingSettings>() ?? new();
      })
      .AddTransient<IRoleSettingsResolver, PortalRoleSettingsResolver>()
      .AddTransient<IUserSettingsResolver, PortalUserSettingsResolver>();
  }
}
