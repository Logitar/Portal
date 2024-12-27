using Logitar.Identity.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  private const string ConfigurationKey = "POSTGRESQLCONNSTR_Portal";

  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = Environment.GetEnvironmentVariable(ConfigurationKey);
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      connectionString = configuration.GetValue<string>(ConfigurationKey);
    }
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException($"The configuration '{ConfigurationKey}' could not be found.", nameof(configuration));
    }
    return services.AddLogitarPortalWithEntityFrameworkCorePostgreSQL(connectionString.Trim());
  }
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<PortalContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Logitar.Portal.EntityFrameworkCore.PostgreSQL")))
      .AddLogitarIdentityWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddSingleton<IQueryHelper, PostgresQueryHelper>();
  }
}
