﻿using Logitar.Portal.Extensions;

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

    services.AddControllers(/*options => options.Filters.Add<PortalExceptionFilterAttribute>()*/)
      .AddJsonOptions(options =>
      {
        //options.JsonSerializerOptions.Converters.Add(new CultureInfoConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
      });

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    services.AddMemoryCache();

    services.AddHttpContextAccessor();
    //services.AddSingleton<IApplicationContext, HttpApplicationContext>();

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? DatabaseProvider.EntityFrameworkCorePostgreSQL;
    string connectionString;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        connectionString = _configuration.GetValue<string>("POSTGRESQLCONNSTR_Portal") ?? string.Empty;
        //services.AddLogitarPortalWithEntityFrameworkCorePostgreSQL(connectionString);
        AddDbContextChecks(healthChecks);
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = _configuration.GetValue<string>("SQLCONNSTR_Portal") ?? string.Empty;
        //services.AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString);
        AddDbContextChecks(healthChecks);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }
  private static void AddDbContextChecks(IHealthChecksBuilder builder)
  {
    //builder.AddDbContextCheck<EventContext>();
    //builder.AddDbContextCheck<IdentityContext>();
    //builder.AddDbContextCheck<PortalContext>();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    builder.UseHttpsRedirection();

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
