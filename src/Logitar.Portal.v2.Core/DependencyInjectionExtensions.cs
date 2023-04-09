﻿using Logitar.Portal.v2.Contracts.Realms;
using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core.Configurations;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Users;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.v2.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalCore(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddFacades()
      .AddMediatR(options => options.RegisterServicesFromAssembly(assembly))
      .AddTransient<IRequestPipeline, RequestPipeline>();
  }

  private static IServiceCollection AddFacades(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationService, ConfigurationService>()
      .AddTransient<IRealmService, RealmService>()
      .AddTransient<IUserService, UserService>();
  }
}
