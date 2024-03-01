﻿namespace Logitar.Portal.Worker;

internal class Program
{
  public static void Main(string[] args)
  {
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    IHost host = builder.Build();
    host.Run();
  }
}
