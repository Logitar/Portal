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

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddLogitarPortalCore();

    services.AddSingleton<ICurrentActor, HttpCurrentActor>();

    return services;
  }
}
