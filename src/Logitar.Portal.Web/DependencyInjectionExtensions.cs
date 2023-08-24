using Logitar.Portal.Application;

namespace Logitar.Portal.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWeb(this IServiceCollection services)
  {
    return services.AddSingleton<IApplicationContext, HttpApplicationContext>();
  }
}
