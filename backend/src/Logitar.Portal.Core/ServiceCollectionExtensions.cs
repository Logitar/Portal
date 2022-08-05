using FluentValidation;
using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Core.Emails.Senders;
using Logitar.Portal.Core.Emails.Templates;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Tokens;
using Logitar.Portal.Core.Users;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Core
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddPortalCore(this IServiceCollection services)
    {
      Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

      return services
        .AddAutoMapper(assembly)
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
        .AddScoped<IMessageHandlerFactory, MessageHandlerFactory>()
        .AddScoped<Realms.Mutations.DeleteRealmMutationHandler>()
        .AddDomainServices();
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
      return services
        .AddScoped<IAccountService, AccountService>()
        .AddScoped<IApiKeyService, ApiKeyService>()
        .AddScoped<IConfigurationService, ConfigurationService>()
        .AddScoped<IGoogleService, GoogleService>()
        .AddScoped<IMessageService, MessageService>()
        .AddScoped<IRealmService, RealmService>()
        .AddScoped<ISenderService, SenderService>()
        .AddScoped<ISessionService, SessionService>()
        .AddScoped<ITemplateService, TemplateService>()
        .AddScoped<ITokenService, TokenService>()
        .AddScoped<IUserService, UserService>();
    }
  }
}
