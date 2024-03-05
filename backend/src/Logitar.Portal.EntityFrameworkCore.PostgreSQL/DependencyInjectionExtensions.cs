﻿using Logitar.Identity.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, IConfiguration configuration)
  {
    string connectionString = configuration.GetValue<string>("POSTGRESQLCONNSTR_Portal") ?? string.Empty;
    return services.AddLogitarPortalWithEntityFrameworkCorePostgreSQL(connectionString);
  }
  public static IServiceCollection AddLogitarPortalWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<PortalContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Logitar.Portal.EntityFrameworkCore.PostgreSQL")))
      .AddLogitarIdentityWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddLogitarPortalWithEntityFrameworkCoreRelational()
      .AddSingleton<ISearchHelper, PostgresSearchHelper>();
  }
}