using Logitar.Identity.EntityFrameworkCore.SqlServer;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  private const string ConfigurationKey = "SQLCONNSTR_Portal";

  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
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
    return services.AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString.Trim());
  }
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<PortalContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Logitar.Portal.EntityFrameworkCore.SqlServer")))
      .AddLogitarIdentityWithEntityFrameworkCoreSqlServer(connectionString)
      .AddLogitarPortalWithEntityFrameworkCoreRelational()
      .AddSingleton<ISearchHelper, SqlServerSearchHelper>();
  }
}
