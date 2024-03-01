namespace Logitar.Portal.MassTransit.Settings;

public record MassTransitSettings : IMassTransitSettings
{
  public TransportBroker TransportBroker { get; set; }

  public RabbitMqSettings? RabbitMQ { get; set; }
}
