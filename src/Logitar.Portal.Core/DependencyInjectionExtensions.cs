using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Dictionaries;
using Logitar.Portal.Core.Messages;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Templates;
using Logitar.Portal.Core.Tokens;
using Logitar.Portal.Core.Users;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalCore(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddAutoMapper(assembly)
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
