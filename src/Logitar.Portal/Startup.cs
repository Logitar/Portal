using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.v2.Web;
using Logitar.Portal.v2.Web.Extensions;
using Logitar.Portal.v2.Web.Middlewares;

namespace Logitar.Portal;

public class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _enableOpenApi = _configuration.GetValue<bool>("EnableOpenApi");
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

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider>("DatabaseProvider");
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddLogitarPortalEntityFrameworkCorePostgreSQLStore(_configuration);
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
    builder.UseMiddleware<Logging>();
    builder.UseMiddleware<RefreshSession>();
    builder.UseMiddleware<RedirectUnauthorized>();

    if (builder is WebApplication application)
    {
      application.MapControllers();
    }
  }
}
