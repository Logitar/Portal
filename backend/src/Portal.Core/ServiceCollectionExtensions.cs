using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portal.Core.Accounts;
using Portal.Core.ApiKeys;
using Portal.Core.Configurations;
using Portal.Core.Emails.Messages;
using Portal.Core.Emails.Senders;
using Portal.Core.Emails.Templates;
using Portal.Core.Realms;
using Portal.Core.Settings;
using Portal.Core.Tokens;
using Portal.Core.Users;
using System.Reflection;

namespace Portal.Core
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddPortalCore(this IServiceCollection services, IConfiguration configuration)
    {
      Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

      IConfigurationSection identity = configuration.GetSection("Identity");
      var userSettings = identity.GetSection("User").Get<UserSettings>() ?? new();

      return services
        .AddAutoMapper(assembly)
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
        .AddSingleton(userSettings)
        .AddScoped<IMessageHandlerFactory, MessageHandlerFactory>()
        .AddDomainServices();
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
      return services
        .AddScoped<IAccountService, AccountService>()
        .AddScoped<IApiKeyService, ApiKeyService>()
        .AddScoped<IConfigurationService, ConfigurationService>()
        .AddScoped<IMessageService, MessageService>()
        .AddScoped<IRealmService, RealmService>()
        .AddScoped<ISenderService, SenderService>()
        .AddScoped<ITemplateService, TemplateService>()
        .AddScoped<ITokenService, TokenService>()
        .AddScoped<IUserService, UserService>();
    }
  }
}
