using Logitar.Portal.Application;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.Web.Authentication;
using Logitar.Portal.Web.Authorization;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Filters;
using Logitar.Portal.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Web
{
  public class Startup : StartupBase
  {
    public override void ConfigureServices(IServiceCollection services)
    {
      base.ConfigureServices(services);

      services.AddControllersWithViews(options => options.Filters.Add(new ExceptionFilterAttribute()))
        .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

      services.AddAuthentication()
        //.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Constants.Schemes.ApiKey, options => { }) // TODO(fpion): implement
        .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Constants.Schemes.Session, options => { });

      services.AddAuthorization(options =>
      {
        //options.AddPolicy(Constants.Policies.ApiKey, new AuthorizationPolicyBuilder(Constants.Schemes.All)
        //  .RequireAuthenticatedUser()
        //  .AddRequirements(new ApiKeyAuthorizationRequirement())
        //  .Build()); // TODO(fpion): implement
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
      services.AddHealthChecks()
        .AddDbContextCheck<PortalContext>();

      services.AddHttpContextAccessor();

      services.AddOpenApi();

      services.AddSession(options =>
      {
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
      }).AddDistributedMemoryCache();

      services.AddLogitarPortalApplication();
      services.AddLogitarPortalInfrastructure();

      //services.AddSingleton<IAuthorizationHandler, ApiKeyAuthorizationHandler>(); // TODO(fpion): implement
      services.AddSingleton<IAuthorizationHandler, PortalIdentityAuthorizationHandler>();
      services.AddSingleton<IAuthorizationHandler, SessionAuthorizationHandler>();
      services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();
      services.AddSingleton<IUserContext, HttpUserContext>();
    }

    public override void Configure(IApplicationBuilder applicationBuilder)
    {
      if (applicationBuilder is WebApplication application)
      {
        if (!application.Environment.IsProduction())
        {
          application.UseOpenApi();
        }

        application.UseHttpsRedirection();
        application.UseStaticFiles();
        application.UseSession();
        //application.UseMiddleware<Logging>(); // TODO(fpion): implement
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
