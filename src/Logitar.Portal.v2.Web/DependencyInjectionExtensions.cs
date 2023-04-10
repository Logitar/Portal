using Logitar.Portal.v2.Core;
using Logitar.Portal.v2.Web.Filters;
using System.Text.Json.Serialization;

namespace Logitar.Portal.v2.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWeb(this IServiceCollection services)
  {
    services.AddControllersWithViews(options => options.Filters.Add<CoreExceptionFilterAttribute>())
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    services
      .AddSession(options =>
      {
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
      })
      .AddDistributedMemoryCache();

    services.AddHttpContextAccessor();

    services.AddLogitarPortalCore();

    services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly));

    services.AddSingleton<ICurrentActor, HttpCurrentActor>();

    return services;
  }
}
