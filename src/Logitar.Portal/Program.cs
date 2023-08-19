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

    // TODO(fpion): initialize caching
    // TODO(fpion): initialize database
    //using IServiceScope scope = application.Services.CreateScope();
    //IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    //if (application.Configuration.GetValue<bool>("EnableMigrations"))
    //{
    //  await mediator.Publish(new InitializeDatabaseCommand());
    //}
    //await mediator.Send(new InitializeCacheCommand());

    application.Run();
  }
}
