using Portal.Infrastructure;

namespace Portal.Web
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

      var startup = new Startup(builder.Configuration);
      startup.ConfigureServices(builder.Services);

      WebApplication application = builder.Build();

      startup.Configure(application);

      using IServiceScope scope = application.Services.CreateScope();
      var databaseService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
      await databaseService.InitializeAsync();

      application.Run();
    }
  }
}
