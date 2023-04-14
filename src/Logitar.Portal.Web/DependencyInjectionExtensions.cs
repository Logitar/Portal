using Logitar.Portal.Core;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.Web.Authentication;
using Logitar.Portal.Web.Authorization;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;

using SharedSchemes = Logitar.Portal.Contracts.Constants.Schemes;

namespace Logitar.Portal.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWeb(this IServiceCollection services)
  {
    services
      .AddControllersWithViews(options =>
      {
        options.Filters.Add<ExceptionHandlingFilter>();
        options.Filters.Add<LoggingFilter>();
      })
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    services
      .AddAuthentication()
      .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(SharedSchemes.Basic, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });

    services.AddAuthorization(options =>
    {
      options.AddPolicy(Policies.AuthenticatedPortalUser, new AuthorizationPolicyBuilder(Schemes.Session)
        .RequireAuthenticatedUser()
        .AddRequirements(new AuthenticatedPortalUserAuthorizationRequirement())
        .Build());
      options.AddPolicy(Policies.PortalActor, new AuthorizationPolicyBuilder(Schemes.All)
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

    return services;
  }
}
