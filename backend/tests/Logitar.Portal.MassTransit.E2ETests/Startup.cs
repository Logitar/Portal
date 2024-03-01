using Logitar.Portal.MassTransit.Caching;
using Logitar.Portal.MassTransit.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.MassTransit;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public virtual void ConfigureServices(IServiceCollection services)
  {
    MassTransitSettings settings = _configuration.GetSection("MassTransit").Get<MassTransitSettings>() ?? new();
    services.AddMassTransit(configurator =>
    {
      switch (settings.TransportBroker)
      {
        case TransportBroker.RabbitMQ:
          if (settings.RabbitMQ == null)
          {
            throw new InvalidOperationException($"The {nameof(settings.RabbitMQ)} is required.");
          }
          configurator.AddMassTransitRabbitMQ(settings.RabbitMQ);
          break;
        default:
          throw new TransportBrokerNotSupportedException(settings.TransportBroker);
      }
      configurator.AddConsumers(Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IConsumer).IsAssignableFrom(type)).ToArray());
    });

    services.AddMemoryCache();
    services.AddSingleton<ICacheService, CacheService>();
  }
}
