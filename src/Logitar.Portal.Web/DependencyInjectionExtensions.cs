using Logitar.Portal.Application;
using Logitar.Portal.Infrastructure.Converters;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWeb(this IServiceCollection services)
  {
    services
     .AddControllers(options =>
     {
       //options.Filters.Add<ExceptionHandlingFilter>(); // TODO(fpion): filters
       //options.Filters.Add<LoggingFilter>(); // TODO(fpion): filters
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
