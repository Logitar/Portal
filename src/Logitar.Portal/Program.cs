using Logitar.EventSourcing;
using Logitar.Portal.Core.Caching.Commands;
using Logitar.Portal.Infrastructure.Commands;
using MediatR;

namespace Logitar.Portal;

public class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseStaticWebAssets();

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    TypeExtensions.DoNotUseFullAssemblyName = true;

    using IServiceScope scope = application.Services.CreateScope();

    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediator.Publish(new InitializeDatabase());
    await mediator.Publish(new InitializeCaching());

    application.Run();
  }
}
