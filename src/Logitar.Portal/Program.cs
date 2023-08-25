using Logitar.Portal.Application.Caching.Commands;
using Logitar.Portal.Infrastructure;
using MediatR;

namespace Logitar.Portal;

internal class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    using IServiceScope scope = application.Services.CreateScope();
    IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());
    await publisher.Publish(new InitializeCachingCommand());

    application.Run();
  }
}
