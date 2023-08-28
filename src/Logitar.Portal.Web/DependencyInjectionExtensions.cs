﻿using Logitar.Portal.Application;
using Logitar.Portal.Infrastructure.Converters;
using Logitar.Portal.Web.Authentication;
using Logitar.Portal.Web.Authorization;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Filters;
using Logitar.Portal.Web.Settings;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Portal.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services
     .AddControllersWithViews(options =>
     {
       options.Filters.Add<ExceptionHandlingFilter>();
       options.Filters.Add<LoggingFilter>();
     })
     .AddJsonOptions(options =>
     {
       options.JsonSerializerOptions.Converters.Add(new LocaleConverter());
       options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
     });

    CorsSettings corsSettings = configuration.GetSection("Cors").Get<CorsSettings>() ?? new();
    services.AddCors(corsSettings);

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
      options.Cookie.SameSite = SameSiteMode.None; // TODO(fpion): Strict
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    services.AddDistributedMemoryCache();
    services.AddMemoryCache();
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();
    services.AddSingleton<IAuthorizationHandler, PortalActorAuthorizationHandler>();

    return services;
  }
}
