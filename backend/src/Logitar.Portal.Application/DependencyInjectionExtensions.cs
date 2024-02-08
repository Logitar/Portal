using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Settings;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
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
      .AddSingleton<IUserSettingsResolver, PortalUserSettingsResolver>()
      .AddTransient<IRealmManager, RealmManager>();
  }

  private static IServiceCollection AddFacades(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationService, ConfigurationFacade>()
      .AddTransient<IApiKeyService, ApiKeyFacade>()
      .AddTransient<IRealmService, RealmFacade>()
      .AddTransient<IRoleService, RoleFacade>()
      .AddTransient<ISessionService, SessionFacade>()
      .AddTransient<IUserService, UserFacade>();
  }
}
