using Logitar.Portal.Worker.Settings;
using MassTransit;

namespace Logitar.Portal.Worker;

internal static class DependencyInjectionExtensions
{
  public static void AddMassTransitRabbitMQ(this IBusRegistrationConfigurator configurator, RabbitMqSettings settings)
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
