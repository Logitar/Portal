﻿using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Authentication;
using Logitar.Portal.Constants;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.SqlServer;
using Logitar.Portal.Extensions;
using Logitar.Portal.Filters;
using Logitar.Portal.Middlewares;

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

    services.AddControllers(options => options.Filters.Add<ExceptionHandlingAttribute>())
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    services
      .AddAuthentication()
      .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });

    //services.AddAuthorization(options =>
    //{
    //  options.AddPolicy(Policies.AuthenticatedPortalUser, new AuthorizationPolicyBuilder(Schemes.Session)
    //    .RequireAuthenticatedUser()
    //    .AddRequirements(new AuthenticatedPortalUserAuthorizationRequirement())
    //    .Build());
    //  options.AddPolicy(Policies.PortalActor, new AuthorizationPolicyBuilder(Schemes.All)
    //    .RequireAuthenticatedUser()
    //    .AddRequirements(new PortalActorAuthorizationRequirement())
    //    .Build());
    //});

    services.AddSession(options =>
    {
      options.Cookie.SameSite = SameSiteMode.Strict;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    services.AddDistributedMemoryCache();
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();
    //services.AddSingleton<IAuthorizationHandler, AuthenticatedPortalUserAuthorizationHandler>();
    //services.AddSingleton<IAuthorizationHandler, PortalActorAuthorizationHandler>();

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    string connectionString;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        connectionString = _configuration.GetValue<string>("POSTGRESQLCONNSTR_Portal") ?? string.Empty;
        services.AddLogitarPortalWithEntityFrameworkCorePostgreSQL(connectionString);
        AddDbContextChecks(healthChecks);
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = _configuration.GetValue<string>("SQLCONNSTR_Portal") ?? string.Empty;
        services.AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString);
        AddDbContextChecks(healthChecks);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }
  private static void AddDbContextChecks(IHealthChecksBuilder builder)
  {
    builder.AddDbContextCheck<EventContext>();
    builder.AddDbContextCheck<IdentityContext>();
    builder.AddDbContextCheck<PortalContext>();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    builder.UseHttpsRedirection();
    builder.UseSession();
    builder.UseMiddleware<RefreshSession>();
    builder.UseAuthentication();
    //builder.UseAuthorization();

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
