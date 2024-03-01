namespace Logitar.Portal.Worker.Settings;

public interface IMassTransitSettings
{
  TransportBroker TransportBroker { get; }

  RabbitMqSettings? RabbitMQ { get; }
}
