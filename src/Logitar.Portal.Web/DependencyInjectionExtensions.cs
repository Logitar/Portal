using Logitar.Portal.Application;
using Logitar.Portal.Infrastructure.Converters;
using Logitar.Portal.Web.Filters;

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

    services.AddSession(options =>
    {
      options.Cookie.SameSite = SameSiteMode.Strict;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    return services
      .AddDistributedMemoryCache()
      .AddMemoryCache()
      .AddSingleton<IApplicationContext, HttpApplicationContext>();
  }
}
