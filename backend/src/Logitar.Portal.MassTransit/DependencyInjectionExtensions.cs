using Logitar.Portal.MassTransit.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.MassTransit;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalMassTransit(this IServiceCollection services, IConfiguration configuration)
  {
    MassTransitSettings settings = configuration.GetSection("MassTransit").Get<MassTransitSettings>() ?? new();
    return services.AddLogitarPortalMassTransit(settings);
  }
  public static IServiceCollection AddLogitarPortalMassTransit(this IServiceCollection services, MassTransitSettings settings)
  {
    if (settings.TransportBroker.HasValue)
    {
      services.AddMassTransit(configurator =>
      {
        switch (settings.TransportBroker.Value)
        {
          case TransportBroker.RabbitMQ:
            configurator.AddMassTransitRabbitMq(settings.RabbitMQ);
            break;
          default:
            throw new TransportBrokerNotSupportedException(settings.TransportBroker.Value);
        }
        configurator.AddConsumers(Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IConsumer).IsAssignableFrom(type)).ToArray());
      });
    }

    return services;
  }

  private static void AddMassTransitRabbitMq(this IBusRegistrationConfigurator configurator, RabbitMqSettings? settings)
  {
    ArgumentNullException.ThrowIfNull(settings);

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
