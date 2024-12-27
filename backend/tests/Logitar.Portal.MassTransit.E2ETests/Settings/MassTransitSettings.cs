namespace Logitar.Portal.MassTransit.Settings;

public record MassTransitSettings : IMassTransitSettings
{
  public const string SectionKey = "MassTransit";

  public TransportBroker TransportBroker { get; set; }

  public RabbitMqSettings? RabbitMQ { get; set; }
}
