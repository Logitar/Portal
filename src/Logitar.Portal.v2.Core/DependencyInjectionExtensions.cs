using Logitar.Portal.v2.Contracts.Dictionaries;
using Logitar.Portal.v2.Contracts.Messages;
using Logitar.Portal.v2.Contracts.Realms;
using Logitar.Portal.v2.Contracts.Senders;
using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Contracts.Templates;
using Logitar.Portal.v2.Contracts.Tokens;
using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core.Configurations;
using Logitar.Portal.v2.Core.Dictionaries;
using Logitar.Portal.v2.Core.Messages;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Senders;
using Logitar.Portal.v2.Core.Sessions;
using Logitar.Portal.v2.Core.Templates;
using Logitar.Portal.v2.Core.Tokens;
using Logitar.Portal.v2.Core.Users;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.v2.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalCore(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddFacades()
      .AddMediatR(options => options.RegisterServicesFromAssembly(assembly))
      .AddTransient<IRequestPipeline, RequestPipeline>()
      .AddTransient<ITokenManager, JsonWebTokenManager>();
  }

  private static IServiceCollection AddFacades(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationService, ConfigurationService>()
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
