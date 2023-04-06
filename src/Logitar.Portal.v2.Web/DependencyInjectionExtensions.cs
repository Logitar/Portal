using Logitar.Portal.v2.Core;
using Logitar.Portal.v2.Web.Filters;
using System.Text.Json.Serialization;

namespace Logitar.Portal.v2.Web;

public static class DependencyInjectionExtensions
{
  /// <summary>
  /// TODO(fpion): rename
  /// </summary>
  /// <param name="services"></param>
  /// <returns></returns>
  public static IServiceCollection AddLogitarPortalv2Web(this IServiceCollection services)
  {
    services.AddControllersWithViews(options => options.Filters.Add<CoreExceptionFilterAttribute>())
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddLogitarPortalV2Core();

    services.AddSingleton<ICurrentActor, HttpCurrentActor>();

    return services;
  }
}
