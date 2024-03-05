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
  public static IServiceCollection AddLogitarPortalMassTransit(this IServiceCollection services, IMassTransitSettings settings)
  {
    if (settings.TransportBroker.HasValue)
    {
      services.AddMassTransit(configurator =>
      {
        switch (settings.TransportBroker.Value)
        {
          case TransportBroker.RabbitMQ:
            configurator.AddMassTransitRabbitMQ(settings.RabbitMQ ?? new());
            break;
          default:
            throw new TransportBrokerNotSupportedException(settings.TransportBroker.Value);
        }
        configurator.AddConsumers(Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IConsumer).IsAssignableFrom(type)).ToArray());
      });
      services.AddScoped<IConsumerPipeline, ConsumerPipeline>();
    }

    return services.AddSingleton(settings);
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
