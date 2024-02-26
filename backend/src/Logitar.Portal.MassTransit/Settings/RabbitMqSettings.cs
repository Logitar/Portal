﻿namespace Logitar.Portal.MassTransit.Settings;

public record RabbitMqSettings
{
  public string Host { get; set; } = "localhost";
  public ushort Port { get; set; } = 5672;
  public string VirtualHost { get; set; } = "/";
  public string Username { get; set; } = "guest";
  public string Password { get; set; } = "guest";
}
