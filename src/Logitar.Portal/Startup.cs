using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.GraphQL;
using Logitar.Portal.Web;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Middlewares;

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

    services.AddLogitarPortalWeb(_configuration);
    services.AddLogitarPortalGraphQL(_configuration);

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

    if (_configuration.GetValue<bool>("UseGraphQLAltair"))
    {
      builder.UseGraphQLAltair();
    }
    if (_configuration.GetValue<bool>("UseGraphQLGraphiQL"))
    {
      builder.UseGraphQLGraphiQL();
    }
    if (_configuration.GetValue<bool>("UseGraphQLPlayground"))
    {
      builder.UseGraphQLPlayground();
    }
    if (_configuration.GetValue<bool>("UseGraphQLVoyager"))
    {
      builder.UseGraphQLVoyager();
    }

    builder.UseHttpsRedirection();
    builder.UseCors();
    builder.UseStaticFiles();
    builder.UseSession();
    builder.UseMiddleware<Logging>();
    builder.UseAuthentication();
    builder.UseAuthorization();

    builder.UseGraphQL<PortalSchema>();

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
