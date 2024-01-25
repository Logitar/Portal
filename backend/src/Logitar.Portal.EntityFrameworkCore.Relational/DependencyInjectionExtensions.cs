using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Repositories;
using Logitar.Portal.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityWithEntityFrameworkCoreRelational()
      .AddLogitarPortalInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddRepositories();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services.AddTransient<IRealmRepository, RealmRepository>();
  }
}
