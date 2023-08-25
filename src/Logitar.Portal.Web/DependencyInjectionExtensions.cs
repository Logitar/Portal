using Logitar.Portal.Application;
using Logitar.Portal.Infrastructure.Converters;
using Logitar.Portal.Web.Authentication;
using Logitar.Portal.Web.Authorization;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Portal.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWeb(this IServiceCollection services)
  {
    services
     .AddControllers(options =>
     {
       options.Filters.Add<ExceptionHandlingFilter>();
       options.Filters.Add<LoggingFilter>();
     })
     .AddJsonOptions(options =>
     {
       options.JsonSerializerOptions.Converters.Add(new LocaleConverter());
       options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
     });

    services.AddAuthentication()
      .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });

    services.AddAuthorization(options =>
    {
      options.AddPolicy(Policies.PortalActor, new AuthorizationPolicyBuilder(Schemes.All)
        .RequireAuthenticatedUser()
        .AddRequirements(new PortalActorAuthorizationRequirement())
        .Build());
    });

    services.AddSession(options =>
    {
      options.Cookie.SameSite = SameSiteMode.Strict;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    services.AddDistributedMemoryCache();
    services.AddMemoryCache();
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();
    services.AddSingleton<IAuthorizationHandler, PortalActorAuthorizationHandler>();

    return services;
  }
}
