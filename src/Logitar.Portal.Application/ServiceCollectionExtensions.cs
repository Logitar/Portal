using FluentValidation;
using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Emails.Messages;
using Logitar.Portal.Application.Emails.Senders;
using Logitar.Portal.Application.Emails.Templates;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Realms.Mutations;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
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
        .AddScoped<IAccountService, AccountService>()
        .AddScoped<IGoogleService, GoogleService>()
        .AddScoped<IApiKeyService, ApiKeyService>()
        .AddScoped<IDictionaryService, DictionaryService>()
        .AddScoped<IMessageService, MessageService>()
        .AddScoped<ISenderService, SenderService>()
        .AddScoped<ITemplateService, TemplateService>()
        .AddScoped<IRealmService, RealmService>()
        .AddScoped<ISessionService, SessionService>()
        .AddScoped<ITokenService, TokenService>()
        .AddScoped<IUserService, UserService>()
        .AddScoped<IConfigurationService, ConfigurationService>();
    }
  }
}
