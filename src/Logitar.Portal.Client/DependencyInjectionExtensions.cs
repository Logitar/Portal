using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
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
      .AddTransient<IDictionaryService, DictionaryService>()
      .AddTransient<IMessageService, MessageService>()
      .AddTransient<IRealmService, RealmService>()
      .AddTransient<ISenderService, SenderService>()
      .AddTransient<ISessionService, SessionService>()
      .AddTransient<ITemplateService, TemplateService>()
      .AddTransient<ITokenService, TokenService>()
      .AddTransient<IUserService, UserService>();
  }
}
