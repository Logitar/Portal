namespace Logitar.Portal.MassTransit.Settings;

public record MassTransitSettings
{
  public TransportBroker? TransportBroker { get; set; }

  public RabbitMqSettings? RabbitMQ { get; set; }
}
