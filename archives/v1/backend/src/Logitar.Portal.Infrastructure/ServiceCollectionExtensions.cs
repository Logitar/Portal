using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Emails.Messages;
using Logitar.Portal.Application.Emails.Providers;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Infrastructure.Actors;
using Logitar.Portal.Infrastructure.Emails.Messages;
using Logitar.Portal.Infrastructure.Queriers;
using Logitar.Portal.Infrastructure.Repositories;
using Logitar.Portal.Infrastructure.Settings;
using Logitar.Portal.Infrastructure.Tokens;
using Logitar.Portal.Infrastructure.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        .AddScoped<IActorService, ActorService>()
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
        .AddScoped<Application.ApiKeys.IApiKeyQuerier, ApiKeyQuerier>()
        .AddScoped<Application.Dictionaries.IDictionaryQuerier, DictionaryQuerier>()
        .AddScoped<IMessageQuerier, MessageQuerier>()
        .AddScoped<Application.Emails.Senders.ISenderQuerier, SenderQuerier>()
        .AddScoped<Application.Emails.Templates.ITemplateQuerier, TemplateQuerier>()
        .AddScoped<Application.Realms.IRealmQuerier, RealmQuerier>()
        .AddScoped<Application.Sessions.ISessionQuerier, SessionQuerier>()
        .AddScoped<IUserQuerier, UserQuerier>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
      return services
        .AddScoped<IRepository<Domain.ApiKeys.ApiKey>, Repository<Domain.ApiKeys.ApiKey>>()
        .AddScoped<IRepository<Domain.Dictionaries.Dictionary>, Repository<Domain.Dictionaries.Dictionary>>()
        .AddScoped<IRepository<Domain.Emails.Messages.Message>, Repository<Domain.Emails.Messages.Message>>()
        .AddScoped<IRepository<Domain.Emails.Senders.Sender>, Repository<Domain.Emails.Senders.Sender>>()
        .AddScoped<IRepository<Domain.Emails.Templates.Template>, Repository<Domain.Emails.Templates.Template>>()
        .AddScoped<IRepository<Domain.Realms.Realm>, Repository<Domain.Realms.Realm>>()
        .AddScoped<IRepository<Domain.Sessions.Session>, Repository<Domain.Sessions.Session>>()
        .AddScoped<IRepository<Domain.Users.User>, Repository<Domain.Users.User>>();
    }
  }
}
