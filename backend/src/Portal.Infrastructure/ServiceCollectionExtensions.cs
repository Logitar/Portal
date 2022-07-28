using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portal.Core;
using Portal.Core.ApiKeys;
using Portal.Core.Realms;
using Portal.Core.Sessions;
using Portal.Core.Templates;
using Portal.Core.Tokens;
using Portal.Core.Users;
using Portal.Infrastructure.Queriers;
using Portal.Infrastructure.Repositories;
using Portal.Infrastructure.Settings;
using Portal.Infrastructure.Tokens;
using Portal.Infrastructure.Users;

namespace Portal.Infrastructure
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddPortalInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
      IConfigurationSection identity = configuration.GetSection("Identity");

      return services
        .AddDbContext<PortalDbContext>()
        .AddSingleton(configuration.GetSection("Jwt").Get<JwtSettings>() ?? new())
        .AddSingleton(identity.GetSection("Password").Get<PasswordSettings>() ?? new())
        .AddSingleton<IPasswordService, PasswordService>()
        .AddScoped<IDatabaseService, DatabaseService>()
        .AddScoped<IJwtBlacklist, JwtBlacklist>()
        .AddScoped<ISecurityTokenService, JwtService>()
        .AddQueriers()
        .AddRepositories();
    }

    private static IServiceCollection AddQueriers(this IServiceCollection services)
    {
      return services
        .AddScoped<IApiKeyQuerier, ApiKeyQuerier>()
        .AddScoped<IRealmQuerier, RealmQuerier>()
        .AddScoped<ISessionQuerier, SessionQuerier>()
        .AddScoped<ITemplateQuerier, TemplateQuerier>()
        .AddScoped<IUserQuerier, UserQuerier>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
      return services
        .AddScoped<IRepository<ApiKey>, Repository<ApiKey>>()
        .AddScoped<IRepository<Realm>, Repository<Realm>>()
        .AddScoped<IRepository<Session>, Repository<Session>>()
        .AddScoped<IRepository<Template>, Repository<Template>>()
        .AddScoped<IRepository<User>, Repository<User>>();
    }
  }
}
