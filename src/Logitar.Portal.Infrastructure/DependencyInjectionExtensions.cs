using Logitar.Portal.Core;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Infrastructure.Caching;
using Logitar.Portal.Infrastructure.Messages;
using Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddLogitarPortalCore()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>()
      .AddStrategies();
  }

  private static IServiceCollection AddStrategies(this IServiceCollection services)
  {
    return services.AddSingleton<ISendGridStrategy, SendGridStrategy>();
  }
}
