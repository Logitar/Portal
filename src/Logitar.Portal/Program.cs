using Logitar.Portal.Application.Caching.Commands;
using Logitar.Portal.Infrastructure;
using MediatR;

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

    using IServiceScope scope = application.Services.CreateScope();
    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    if (application.Configuration.GetValue<bool>("EnableMigrations"))
    {
      await mediator.Publish(new InitializeDatabaseCommand());
    }
    await mediator.Send(new InitializeCacheCommand());

    application.Run();
  }
}
