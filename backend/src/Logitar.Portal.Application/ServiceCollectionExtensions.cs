using FluentValidation;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Emails.Messages;
using Logitar.Portal.Application.Realms.Mutations;
using Logitar.Portal.Application.Sessions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Application
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddPortalApplication(this IServiceCollection services)
    {
      Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

      return services
        .AddAutoMapper(assembly)
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
        .AddScoped<IMappingService, MappingService>()
        .AddScoped<IMessageHandlerFactory, MessageHandlerFactory>()
        .AddScoped<DeleteRealmMutationHandler>()
        .AddScoped<ISignInService, SignInService>()
        .AddApplicationServices();
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      return services
        .AddScoped<Core.Accounts.IAccountService, Accounts.AccountService>()
        .AddScoped<Core.Accounts.IGoogleService, Accounts.GoogleService>()
        .AddScoped<Core.ApiKeys.IApiKeyService, ApiKeys.ApiKeyService>()
        .AddScoped<Core.Dictionaries.IDictionaryService, Dictionaries.DictionaryService>()
        .AddScoped<Core.Emails.Messages.IMessageService, MessageService>()
        .AddScoped<Core.Emails.Senders.ISenderService, Emails.Senders.SenderService>()
        .AddScoped<Core.Emails.Templates.ITemplateService, Emails.Templates.TemplateService>()
        .AddScoped<Core.Realms.IRealmService, Realms.RealmService>()
        .AddScoped<Core.Sessions.ISessionService, SessionService>()
        .AddScoped<Core.Tokens.ITokenService, Tokens.TokenService>()
        .AddScoped<Core.Users.IUserService, Users.UserService>()
        .AddScoped<IConfigurationService, ConfigurationService>();
    }
  }
}
