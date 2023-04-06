using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Queriers;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  /// <summary>
  /// TODO(fpion): rename
  /// </summary>
  /// <param name="services"></param>
  /// <param name="connectionString"></param>
  /// <returns></returns>
  public static IServiceCollection AddLogitarPortalv2EntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddAutoMapper(assembly)
      .AddDbContext<PortalContext>(options => options.UseNpgsql(connectionString))
      .AddEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddQueriers()
      .AddRepositories()
      .AddScoped<IActorService, ActorService>()
      .AddScoped<IEventBus, EventBus>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services.AddScoped<IRealmQuerier, RealmQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services.AddScoped<IRealmRepository, RealmRepository>();
  }
}
