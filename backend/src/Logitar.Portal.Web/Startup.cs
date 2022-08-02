using Microsoft.AspNetCore.Authorization;
using Logitar.Portal.Core;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.Web.Authentication;
using Logitar.Portal.Web.Authorization;
using Logitar.Portal.Web.Filters;
using Logitar.Portal.Web.Middlewares;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Web
{
  internal class Startup : StartupBase
  {
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public override void ConfigureServices(IServiceCollection services)
    {
      base.ConfigureServices(services);

      services
        .AddControllersWithViews(options =>
        {
          options.Filters.Add<ApiExceptionFilterAttribute>();
          options.Filters.Add<ValidationExceptionFilterAttribute>();
        })
        .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

      services
        .AddAuthentication()
        .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Constants.Schemes.ApiKey, options => { })
        .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Constants.Schemes.Session, options => { });

      services.AddAuthorization(options =>
      {
        options.AddPolicy(Constants.Policies.ApiKey, new AuthorizationPolicyBuilder(Constants.Schemes.All)
          .RequireAuthenticatedUser()
          .AddRequirements(new ApiKeyAuthorizationRequirement())
          .Build());
        options.AddPolicy(Constants.Policies.AuthenticatedUser, new AuthorizationPolicyBuilder(Constants.Schemes.All)
          .RequireAuthenticatedUser()
          .AddRequirements(new UserAuthorizationRequirement())
          .Build());
        options.AddPolicy(Constants.Policies.PortalIdentity, new AuthorizationPolicyBuilder(Constants.Schemes.All)
          .RequireAuthenticatedUser()
          .AddRequirements(new PortalIdentityAuthorizationRequirement())
          .Build());
        options.AddPolicy(Constants.Policies.Session, new AuthorizationPolicyBuilder(Constants.Schemes.All)
          .RequireAuthenticatedUser()
          .AddRequirements(new SessionAuthorizationRequirement())
          .Build());
      });

      services.AddApplicationInsightsTelemetry();
      services
        .AddHealthChecks()
        .AddDbContextCheck<PortalDbContext>();

      services.AddHttpContextAccessor();

      services.AddOpenApi();

      services
        .AddSession(options =>
        {
          options.Cookie.SameSite = SameSiteMode.Strict;
          options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        })
        .AddDistributedMemoryCache();

      services.AddPortalCore();
      services.AddPortalInfrastructure(_configuration);

      services.AddSingleton<IAuthorizationHandler, ApiKeyAuthorizationHandler>();
      services.AddSingleton<IAuthorizationHandler, PortalIdentityAuthorizationHandler>();
      services.AddSingleton<IAuthorizationHandler, SessionAuthorizationHandler>();
      services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();
      services.AddSingleton<IUserContext, HttpUserContext>();
    }

    public override void Configure(IApplicationBuilder applicationBuilder)
    {
      if (applicationBuilder is WebApplication application)
      {
        if (application.Environment.IsDevelopment())
        {
          application.UseOpenApi();
        }

        application.UseHttpsRedirection();
        application.UseStaticFiles();
        application.UseSession();
        application.UseMiddleware<RenewSession>();
        application.UseMiddleware<RedirectUnauthorized>();
        application.UseAuthentication();
        application.UseAuthorization();
        application.MapControllers();
        application.MapHealthChecks("/health");
      }
    }
  }
}
