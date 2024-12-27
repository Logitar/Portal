using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Infrastructure.Commands;
using MediatR;
using Microsoft.FeatureManagement;

namespace Logitar.Portal;

public class Program
{
  private const string DefaultUniqueName = "portal";
  private const string DefaultPassword = "P@s$W0rD";

  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfiguration configuration = builder.Configuration;

    Startup startup = new(configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    IServiceScope scope = application.Services.CreateScope();
    IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();
    if (await featureManager.IsEnabledAsync(FeatureFlags.MigrateDatabase))
    {
      await publisher.Publish(new InitializeDatabaseCommand());
    }

    string uniqueName = configuration.GetValue<string>("PORTAL_USERNAME") ?? DefaultUniqueName;
    string password = configuration.GetValue<string>("PORTAL_PASSWORD") ?? DefaultPassword;
    await publisher.Publish(new InitializeConfigurationCommand(uniqueName, password));

    application.Run();
  }
}
