using Logitar.Portal.Client.ApiKeys;
using Logitar.Portal.Client.Configurations;
using Logitar.Portal.Client.Passwords;
using Logitar.Portal.Client.Realms;
using Logitar.Portal.Client.Roles;
using Logitar.Portal.Client.Sessions;
using Logitar.Portal.Client.Templates;
using Logitar.Portal.Client.Tokens;
using Logitar.Portal.Client.Users;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
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
      .AddSingleton<IApiKeyClient, ApiKeyClient>()
      .AddSingleton<IConfigurationClient, ConfigurationClient>()
      .AddSingleton<IOneTimePasswordClient, OneTimePasswordClient>()
      .AddSingleton<IRealmClient, RealmClient>()
      .AddSingleton<IRoleClient, RoleClient>()
      .AddSingleton<ISessionClient, SessionClient>()
      .AddSingleton<ITemplateClient, TemplateClient>()
      .AddSingleton<ITokenClient, TokenClient>()
      .AddSingleton<IUserClient, UserClient>();
  }
}
