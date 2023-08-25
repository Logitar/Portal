using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<PortalContext>(options => options.UseNpgsql(connectionString,
        builder => builder.MigrationsAssembly("Logitar.Portal.EntityFrameworkCore.PostgreSQL")
      ))
      .AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddLogitarPortalWithEntityFrameworkCoreRelational();
  }
}
