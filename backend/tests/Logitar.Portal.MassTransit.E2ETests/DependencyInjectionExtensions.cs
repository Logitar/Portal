using Logitar.Portal.MassTransit.Settings;
using MassTransit;

namespace Logitar.Portal.MassTransit;

internal static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalMassTransit(this IServiceCollection services, IConfiguration configuration)
  {
    MassTransitSettings settings = configuration.GetSection(MassTransitSettings.SectionKey).Get<MassTransitSettings>() ?? new();
    return services.AddLogitarPortalMassTransit(settings);
  }
  public static IServiceCollection AddLogitarPortalMassTransit(this IServiceCollection services, IMassTransitSettings settings)
  {
    services.AddMassTransit(configurator =>
    {
      switch (settings.TransportBroker)
      {
        case TransportBroker.RabbitMQ:
          configurator.AddMassTransitRabbitMQ(settings.RabbitMQ ?? new());
          break;
        default:
          throw new TransportBrokerNotSupportedException(settings.TransportBroker);
      }
      configurator.AddConsumers(Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IConsumer).IsAssignableFrom(type)).ToArray());
    });

    return services;
  }

  private static void AddMassTransitRabbitMQ(this IBusRegistrationConfigurator configurator, RabbitMqSettings settings)
  {
    configurator.UsingRabbitMq((context, config) =>
    {
      config.Host(settings.Host, settings.Port, settings.VirtualHost, h =>
      {
        h.Username(settings.Username);
        h.Password(settings.Password);
      });
      config.ConfigureEndpoints(context);
    });
  }
}
