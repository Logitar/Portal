using Logitar.Portal.Application;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Filters;
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

      //services.AddAuthentication()
      //  .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Constants.Schemes.ApiKey, options => { })
      //  .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Constants.Schemes.Session, options => { }); // TODO(fpion): implement

      //services.AddAuthorization(options =>
      //{
      //  options.AddPolicy(Constants.Policies.ApiKey, new AuthorizationPolicyBuilder(Constants.Schemes.All)
      //    .RequireAuthenticatedUser()
      //    .AddRequirements(new ApiKeyAuthorizationRequirement())
      //    .Build());
      //  options.AddPolicy(Constants.Policies.AuthenticatedUser, new AuthorizationPolicyBuilder(Constants.Schemes.All)
      //    .RequireAuthenticatedUser()
      //    .AddRequirements(new UserAuthorizationRequirement())
      //    .Build());
      //  options.AddPolicy(Constants.Policies.PortalIdentity, new AuthorizationPolicyBuilder(Constants.Schemes.All)
      //    .RequireAuthenticatedUser()
      //    .AddRequirements(new PortalIdentityAuthorizationRequirement())
      //    .Build());
      //  options.AddPolicy(Constants.Policies.Session, new AuthorizationPolicyBuilder(Constants.Schemes.All)
      //    .RequireAuthenticatedUser()
      //    .AddRequirements(new SessionAuthorizationRequirement())
      //    .Build());
      //}); // TODO(fpion): implement

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
        //application.UseMiddleware<RenewSession>(); // TODO(fpion): implement
        //application.UseMiddleware<RedirectUnauthorized>(); // TODO(fpion): implement
        application.UseAuthentication();
        application.UseAuthorization();
        application.MapControllers();
        application.MapHealthChecks("/health");
      }
    }
  }
}
