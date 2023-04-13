using Logitar.Portal.Core;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.Web.Authentication;
using Logitar.Portal.Web.Authorization;
using Logitar.Portal.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWeb(this IServiceCollection services)
  {
    services.AddControllersWithViews(options => options.Filters.Add<CoreExceptionFilterAttribute>())
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    services
      .AddAuthentication()
      .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Constants.Schemes.Basic, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Constants.Schemes.Session, options => { });

    services.AddAuthorization(options =>
    {
      options.AddPolicy(Constants.Policies.AuthenticatedPortalUser, new AuthorizationPolicyBuilder(Constants.Schemes.Session)
        .RequireAuthenticatedUser()
        .AddRequirements(new AuthenticatedPortalUserAuthorizationRequirement())
        .Build());
      options.AddPolicy(Constants.Policies.PortalActor, new AuthorizationPolicyBuilder(Constants.Schemes.All)
        .RequireAuthenticatedUser()
        .AddRequirements(new PortalActorAuthorizationRequirement())
        .Build());
    });

    services
      .AddSession(options =>
      {
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
      })
      .AddDistributedMemoryCache();

    services.AddHttpContextAccessor();

    services.AddLogitarPortalInfrastructure();

    services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly));

    services.AddSingleton<IApplicationContext, HttpApplicationContext>();
    services.AddSingleton<IAuthorizationHandler, AuthenticatedPortalUserAuthorizationHandler>();
    services.AddSingleton<IAuthorizationHandler, PortalActorAuthorizationHandler>();
    services.AddSingleton<ICurrentActor, HttpCurrentActor>();

    return services;
  }
}
