using Logitar.Portal.MassTransit.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
            if (settings.RabbitMQ == null)
            {
              throw new ArgumentException($"The {nameof(settings.RabbitMQ)} is required.", nameof(settings));
            }
            configurator.AddMassTransitRabbitMQ(settings.RabbitMQ);
            break;
          default:
            throw new TransportBrokerNotSupportedException(settings.TransportBroker.Value);
        }
        configurator.AddConsumers(Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IConsumer).IsAssignableFrom(type)).ToArray());
      });
      services.AddScoped<IPopulateRequest, PopulateRequest>();
    }

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
