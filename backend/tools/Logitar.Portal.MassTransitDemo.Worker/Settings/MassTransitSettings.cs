namespace Logitar.Portal.MassTransitDemo.Worker.Settings;

public record MassTransitSettings
{
  public TransportBroker? TransportBroker { get; set; }

  public RabbitMqSettings? RabbitMQ { get; set; }
}
