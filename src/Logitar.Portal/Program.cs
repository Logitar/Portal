using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal;

public class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    // TODO(fpion): refactor
    if (application.Configuration.GetValue<bool>("MigrateDatabase"))
    {
      using IServiceScope scope = application.Services.CreateScope();

      using EventContext events = scope.ServiceProvider.GetRequiredService<EventContext>();
      await events.Database.MigrateAsync();

      using PortalContext portal = scope.ServiceProvider.GetRequiredService<PortalContext>();
      await portal.Database.MigrateAsync();
    }

    application.Run();
  }
}
