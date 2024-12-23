﻿using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Authentication;
using Logitar.Portal.Authorization;
using Logitar.Portal.Constants;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.SqlServer;
using Logitar.Portal.Extensions;
using Logitar.Portal.GraphQL;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.MassTransit;
using Logitar.Portal.Middlewares;
using Logitar.Portal.MongoDB;
using Logitar.Portal.Settings;
using Logitar.Portal.Web;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Portal;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly string[] _authenticationSchemes;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _authenticationSchemes = Schemes.GetEnabled(configuration); // TODO(fpion): FeatureManagement
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi"); // TODO(fpion): FeatureManagement
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddLogitarPortalGraphQL(_configuration);
    services.AddLogitarPortalMassTransit(_configuration);
    services.AddLogitarPortalWeb();

    CorsSettings corsSettings = _configuration.GetSection("Cors").Get<CorsSettings>() ?? new(); // TODO(fpion): SectionKey
    services.AddSingleton(corsSettings);
    services.AddCors(corsSettings);

    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (_authenticationSchemes.Contains(Schemes.Basic))
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }

    AuthorizationPolicy portalActorPolicy = new AuthorizationPolicyBuilder(_authenticationSchemes)
      .RequireAuthenticatedUser()
      .AddRequirements(new PortalActorAuthorizationRequirement())
      .Build();
    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(portalActorPolicy)
      .AddPolicy(Policies.PortalActor, portalActorPolicy)
      .AddPolicy(Policies.PortalUser, new AuthorizationPolicyBuilder(_authenticationSchemes)
        .RequireAuthenticatedUser()
        .AddRequirements(new PortalUserAuthorizationRequirement())
        .Build()
      );
    services.AddSingleton<IAuthorizationHandler, PortalActorAuthorizationHandler>();
    services.AddSingleton<IAuthorizationHandler, PortalUserAuthorizationHandler>();

    CookiesSettings cookiesSettings = _configuration.GetSection("Cookies").Get<CookiesSettings>() ?? new(); // TODO(fpion): SectionKey
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddLogitarPortalWithEntityFrameworkCorePostgreSQL(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<IdentityContext>();
        healthChecks.AddDbContextCheck<PortalContext>();
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarPortalWithEntityFrameworkCoreSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<IdentityContext>();
        healthChecks.AddDbContextCheck<PortalContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
    services.AddLogitarPortalMongoDB(_configuration); // NOTE(fpion): needs to be placed after the relational database to override the LogRepository if MongoDB settings are provided.

    services.AddDistributedMemoryCache();
    services.AddSingleton<IBaseUrl, HttpBaseUrl>();
    services.AddSingleton<IContextParametersResolver, HttpContextParametersResolver>();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi) // TODO(fpion): FeatureManagement
    {
      builder.UseOpenApi();
    }

    if (_configuration.GetValue<bool>("UseGraphQLGraphiQL")) // TODO(fpion): FeatureManagement
    {
      builder.UseGraphQLGraphiQL();
    }
    if (_configuration.GetValue<bool>("UseGraphQLVoyager")) // TODO(fpion): FeatureManagement
    {
      builder.UseGraphQLVoyager();
    }

    builder.UseHttpsRedirection();
    builder.UseCors();
    builder.UseStaticFiles();
    builder.UseSession();
    builder.UseMiddleware<Logging>();
    builder.UseMiddleware<RenewSession>();
    builder.UseMiddleware<RedirectNotFound>();
    builder.UseAuthentication();
    builder.UseAuthorization();

    builder.UseGraphQL<PortalSchema>("/graphql", options => options.AuthenticationSchemes.AddRange(_authenticationSchemes));

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
