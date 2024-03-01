namespace Logitar.Portal.MassTransit.Settings;

public interface IMassTransitSettings
{
  TransportBroker TransportBroker { get; }

  RabbitMqSettings? RabbitMQ { get; }
}
