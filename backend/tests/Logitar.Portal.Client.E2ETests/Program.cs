using Logitar.Portal.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Client;

internal class Program
{
  static async Task Main(string[] args)
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .AddUserSecrets("b2b773e6-c1b2-4500-bcfa-a92c3e545a9c")
      .AddCommandLine(args)
      .Build();
    IServiceProvider serviceProvider = CreateServiceProvider(configuration);

    // TODO(fpion): ApiKey
    // TODO(fpion): Realm

    Console.Write("Press a key to start the end-to-end tests.");
    Console.ReadKey(intercept: true);
    Console.WriteLine();
    Console.WriteLine();

    TestContext context = TestContext.Start(count: 1 + 3);

    InitializeConfigurationTests initializeTests = serviceProvider.GetRequiredService<InitializeConfigurationTests>();
    if (!await initializeTests.ExecuteAsync(context)) // 1 test
    {
      context.End();
      return;
    }

    ConfigurationClientTests configurationTests = serviceProvider.GetRequiredService<ConfigurationClientTests>();
    if (!await configurationTests.ExecuteAsync(context)) // 3 tests
    {
      context.End();
      return;
    }

    // TODO(fpion): implement

    context.End();

    Console.Write("Press a key to close the console.");
    Console.ReadKey(intercept: true);
  }

  static ServiceProvider CreateServiceProvider(IConfiguration configuration)
  {
    ServiceCollection services = new();
    services.AddSingleton(configuration);

    Startup startup = new(configuration);
    startup.ConfigureServices(services);

    return services.BuildServiceProvider();
  }
}
