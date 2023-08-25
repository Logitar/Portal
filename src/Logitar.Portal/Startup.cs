using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.Web;
using Logitar.Portal.Web.Extensions;

namespace Logitar.Portal;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddLogitarPortalWeb();

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    string connectionString;
    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? DatabaseProvider.EntityFrameworkCorePostgreSQL;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        connectionString = _configuration.GetValue<string>("POSTGRESQLCONNSTR_Portal") ?? string.Empty;
        services.AddLogitarPortalWithEntityFrameworkCorePostgreSQL(connectionString);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<PortalContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    builder.UseHttpsRedirection();
    builder.UseStaticFiles();
    builder.UseSession();
    builder.UseAuthentication();
    builder.UseAuthorization();

    if (builder is WebApplication application)
    {
      application.MapControllers();
    }
  }
}
