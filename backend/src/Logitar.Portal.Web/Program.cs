
using Logitar.Portal.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Web
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

      Startup startup = new();
      startup.ConfigureServices(builder.Services);

      WebApplication application = builder.Build();

      startup.Configure(application);

      if (application.Configuration.GetValue<bool>("MigrateDatabase"))
      {
        using IServiceScope scope = application.Services.CreateScope();
        using PortalContext context = scope.ServiceProvider.GetRequiredService<PortalContext>();
        await context.Database.MigrateAsync();
      }

      application.Run();
    }
  }
}
