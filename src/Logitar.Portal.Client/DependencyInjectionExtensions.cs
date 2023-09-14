using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Senders;
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
      .AddTransient<IApiKeyService, ApiKeyClient>()
      .AddTransient<IConfigurationService, ConfigurationClient>()
      .AddTransient<IDictionaryService, DictionaryClient>()
      .AddTransient<IMessageService, MessageClient>()
      .AddTransient<IRealmService, RealmClient>()
      .AddTransient<IRoleService, RoleClient>()
      .AddTransient<ISenderService, SenderClient>()
      .AddTransient<ISessionService, SessionClient>()
      .AddTransient<ITemplateService, TemplateClient>()
      .AddTransient<ITokenService, TokenClient>()
      .AddTransient<IUserService, UserClient>();
  }
}
