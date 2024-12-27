using Logitar.EventSourcing.EntityFrameworkCore.Relational;
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
using Microsoft.FeatureManagement;

namespace Logitar.Portal;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly string[] _authenticationSchemes;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _authenticationSchemes = Schemes.GetEnabled(configuration);
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddLogitarPortalApplication();
    services.AddLogitarPortalInfrastructure();
    services.AddLogitarPortalGraphQL(_configuration);
    services.AddLogitarPortalMassTransit(_configuration);
    services.AddLogitarPortalWeb();

    CorsSettings corsSettings = _configuration.GetSection(CorsSettings.SectionKey).Get<CorsSettings>() ?? new();
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

    CookiesSettings cookiesSettings = _configuration.GetSection(CookiesSettings.SectionKey).Get<CookiesSettings>() ?? new();
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    services.AddOpenApi();

    services.AddLogitarPortalWithEntityFrameworkCoreRelational();
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
    if (builder is WebApplication application)
    {
      ConfigureAsync(application).Wait();
    }
  }
  public virtual async Task ConfigureAsync(WebApplication application)
  {
    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();

    if (await featureManager.IsEnabledAsync(FeatureFlags.UseOpenApi))
    {
      application.UseOpenApi();
    }

    if (await featureManager.IsEnabledAsync(FeatureFlags.UseGraphQLGraphiQL))
    {
      application.UseGraphQLGraphiQL();
    }
    if (await featureManager.IsEnabledAsync(FeatureFlags.UseGraphQLVoyager))
    {
      application.UseGraphQLVoyager();
    }

    application.UseHttpsRedirection();
    application.UseCors();
    application.UseStaticFiles();
    application.UseSession();
    application.UseMiddleware<Logging>();
    application.UseMiddleware<RenewSession>();
    application.UseMiddleware<RedirectNotFound>();
    application.UseAuthentication();
    application.UseAuthorization();

    application.UseGraphQL<PortalSchema>("/graphql", options => options.AuthenticationSchemes.AddRange(_authenticationSchemes));

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}
