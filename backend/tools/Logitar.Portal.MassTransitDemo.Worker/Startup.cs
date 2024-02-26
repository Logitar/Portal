using Logitar.Portal.MassTransitDemo.Worker.Caching;
using Logitar.Portal.MassTransitDemo.Worker.Settings;
using MassTransit;
using System.Reflection;

namespace Logitar.Portal.MassTransitDemo.Worker;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public virtual void ConfigureServices(IServiceCollection services)
  {
    services.AddHostedService<Worker>();
    services.AddMemoryCache();
    services.AddSingleton<ICacheService, CacheService>();

    MassTransitSettings settings = _configuration.GetSection("MassTransit").Get<MassTransitSettings>() ?? new();
    if (settings.TransportBroker.HasValue)
    {
      AddMassTransit(services, settings);
    }
  }

  private static void AddMassTransit(IServiceCollection services, MassTransitSettings settings)
  {
    services.AddMassTransit(configurator =>
    {
      switch (settings.TransportBroker!.Value)
      {
        case TransportBroker.RabbitMQ:
          if (settings.RabbitMQ == null)
          {
            throw new ArgumentException($"The {nameof(settings.RabbitMQ)} is required.", nameof(settings));
          }
          AddMassTransitRabbitMq(configurator, settings.RabbitMQ);
          break;
        default:
          throw new TransportBrokerNotSupportedException(settings.TransportBroker.Value);
      }
      configurator.AddConsumers(Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IConsumer).IsAssignableFrom(type)).ToArray());
    });
  }
  private static void AddMassTransitRabbitMq(IBusRegistrationConfigurator configurator, RabbitMqSettings settings)
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
