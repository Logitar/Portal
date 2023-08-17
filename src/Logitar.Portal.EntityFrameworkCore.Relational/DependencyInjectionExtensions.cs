using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Queriers;
using Logitar.Portal.EntityFrameworkCore.Relational.Repositories;
using Logitar.Portal.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddAutoMapper(assembly)
      .AddLogitarIdentityWithEntityFrameworkCoreRelational()
      .AddLogitarPortalInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddQueriers()
      .AddRepositories();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services.AddScoped<IRealmQuerier, RealmQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IConfigurationRepository, ConfigurationRepository>()
      .AddScoped<IRealmRepository, RealmRepository>();
  }
}
