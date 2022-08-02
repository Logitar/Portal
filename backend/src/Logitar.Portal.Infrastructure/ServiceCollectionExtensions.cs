using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Logitar.Portal.Core;
using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Core.Emails.Providers;
using Logitar.Portal.Core.Emails.Senders;
using Logitar.Portal.Core.Emails.Templates;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Tokens;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Infrastructure.Emails.Messages;
using Logitar.Portal.Infrastructure.Queriers;
using Logitar.Portal.Infrastructure.Repositories;
using Logitar.Portal.Infrastructure.Settings;
using Logitar.Portal.Infrastructure.Tokens;
using Logitar.Portal.Infrastructure.Users;
using System.Reflection;

using Providers = Logitar.Portal.Infrastructure.Emails.Providers;

namespace Logitar.Portal.Infrastructure
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddPortalInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
      Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

      return services
        .AddDbContext<PortalDbContext>()
        .AddSingleton(configuration.GetSection("Jwt").Get<JwtSettings>() ?? new())
        .AddSingleton<IPasswordService, PasswordService>()
        .AddSingleton<ITemplateCompiler, TemplateCompiler>()
        .AddScoped<IDatabaseService, DatabaseService>()
        .AddScoped<IJwtBlacklist, JwtBlacklist>()
        .AddScoped<ISecurityTokenService, JwtService>()
        .AddProviderStrategies()
        .AddQueriers()
        .AddRepositories()
        .AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
    }

    private static IServiceCollection AddProviderStrategies(this IServiceCollection services)
    {
      return services
        .AddScoped<ISendGridStrategy, Providers.SendGrid.SendGridStrategy>();
    }

    private static IServiceCollection AddQueriers(this IServiceCollection services)
    {
      return services
        .AddScoped<IApiKeyQuerier, ApiKeyQuerier>()
        .AddScoped<IMessageQuerier, MessageQuerier>()
        .AddScoped<IRealmQuerier, RealmQuerier>()
        .AddScoped<ISenderQuerier, SenderQuerier>()
        .AddScoped<ISessionQuerier, SessionQuerier>()
        .AddScoped<ITemplateQuerier, TemplateQuerier>()
        .AddScoped<IUserQuerier, UserQuerier>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
      return services
        .AddScoped<IRepository<ApiKey>, Repository<ApiKey>>()
        .AddScoped<IRepository<Message>, Repository<Message>>()
        .AddScoped<IRepository<Realm>, Repository<Realm>>()
        .AddScoped<IRepository<Sender>, Repository<Sender>>()
        .AddScoped<IRepository<Session>, Repository<Session>>()
        .AddScoped<IRepository<Template>, Repository<Template>>()
        .AddScoped<IRepository<User>, Repository<User>>();
    }
  }
}
