using Logitar.Portal.v2.Core;
using Logitar.Portal.v2.Infrastructure.Messages;
using Logitar.Portal.v2.Infrastructure.Messages.Providers.SendGrid;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Portal.v2.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalInfrastructure(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddLogitarPortalCore()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>()
      .AddStrategies();
  }

  private static IServiceCollection AddStrategies(this IServiceCollection services)
  {
    return services.AddSingleton<ISendGridStrategy, SendGridStrategy>();
  }
}
