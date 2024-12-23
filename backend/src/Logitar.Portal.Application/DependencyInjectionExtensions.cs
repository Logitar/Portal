using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.OneTimePasswords;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
  {
    return services
      .AddFacades()
      .AddLogitarIdentityCore()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddScoped<IActivityPipeline, ActivityPipeline>()
      .AddScoped<ILoggingService, LoggingService>()
      .AddTransient<IDictionaryManager, DictionaryManager>()
      .AddTransient<IRealmManager, RealmManager>()
      .AddTransient<ITemplateManager, TemplateManager>();
  }

  private static IServiceCollection AddFacades(this IServiceCollection services)
  {
    return services
      .AddTransient<IApiKeyService, ApiKeyFacade>()
      .AddTransient<IConfigurationService, ConfigurationFacade>()
      .AddTransient<IDictionaryService, DictionaryFacade>()
      .AddTransient<IMessageService, MessageFacade>()
      .AddTransient<IOneTimePasswordService, OneTimePasswordFacade>()
      .AddTransient<IRealmService, RealmFacade>()
      .AddTransient<IRoleService, RoleFacade>()
      .AddTransient<ISenderService, SenderFacade>()
      .AddTransient<ISessionService, SessionFacade>()
      .AddTransient<ITemplateService, TemplateFacade>()
      .AddTransient<ITokenService, TokenFacade>()
      .AddTransient<IUserService, UserFacade>();
  }
}
