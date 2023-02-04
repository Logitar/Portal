using FluentValidation;
using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Application
{
  public static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddLogitarPortalApplication(this IServiceCollection services)
    {
      Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

      return services
        .AddApplicationServices()
        .AddMediatR(assembly)
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
        .AddScoped<IMessageHandlerFactory, MessageHandlerFactory>()
        .AddTransient<IInternalMessageService, InternalMessageService>()
        .AddTransient<IInternalTokenService, InternalTokenService>()
        .AddTransient<ISignInService, SignInService>();
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      return services
        .AddTransient<IAccountService, AccountService>()
        .AddTransient<IApiKeyService, ApiKeyService>()
        .AddTransient<IConfigurationService, ConfigurationService>()
        .AddTransient<IDictionaryService, DictionaryService>()
        .AddTransient<IGoogleService, GoogleService>()
        .AddTransient<IMessageService, MessageService>()
        .AddTransient<IRealmService, RealmService>()
        .AddTransient<ISenderService, SenderService>()
        .AddTransient<ISessionService, SessionService>()
        .AddTransient<ITemplateService, TemplateService>()
        .AddTransient<ITokenService, TokenService>()
        .AddTransient<IUserService, UserService>();
    }
  }
}
