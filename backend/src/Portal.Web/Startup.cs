using Microsoft.AspNetCore.Authorization;
using Portal.Core;
using Portal.Infrastructure;
using Portal.Web.Authentication;
using Portal.Web.Authorization;
using Portal.Web.Filters;
using Portal.Web.Middlewares;
using System.Text.Json.Serialization;

namespace Portal.Web
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
        .AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilterAttribute>())
        .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

      services
        .AddAuthentication()
        .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Constants.Schemes.ApiKey, options => { })
        .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Constants.Schemes.Session, options => { });

      services.AddAuthorization(options =>
      {
        options.AddPolicy(Constants.Policies.AuthenticatedUser, new AuthorizationPolicyBuilder(Constants.Schemes.All)
          .RequireAuthenticatedUser()
          .AddRequirements(new UserAuthorizationRequirement())
          .Build()); // TODO(fpion): 401 Unauthorized => redirect to /account/sign-in
        options.AddPolicy(Constants.Policies.PortalIdentity, new AuthorizationPolicyBuilder(Constants.Schemes.All)
          .RequireAuthenticatedUser()
          .AddRequirements(new PortalIdentityAuthorizationRequirement())
          .Build()); // TODO(fpion): 401 Unauthorized => redirect to /account/sign-in
      });

      services.AddHttpContextAccessor();

      services.AddOpenApi();

      services
        .AddSession(options =>
        {
          options.Cookie.SameSite = SameSiteMode.Strict;
          options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        })
        .AddDistributedMemoryCache();

      services.AddPortalCore(_configuration);
      services.AddPortalInfrastructure(_configuration);

      services.AddSingleton<IAuthorizationHandler, PortalIdentityAuthorizationHandler>();
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
        application.UseAuthentication();
        application.UseAuthorization();
        application.MapControllers();
      }
    }
  }
}
