using Logitar.Identity.EntityFrameworkCore.SqlServer;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
  {
    string connectionString = configuration.GetValue<string>("SQLCONNSTR_Portal") ?? string.Empty;
    return services.AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString);
  }
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<PortalContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Logitar.Portal.EntityFrameworkCore.SqlServer")))
      .AddLogitarIdentityWithEntityFrameworkCoreSqlServer(connectionString)
      .AddLogitarPortalWithEntityFrameworkCoreRelational();
  }
}
