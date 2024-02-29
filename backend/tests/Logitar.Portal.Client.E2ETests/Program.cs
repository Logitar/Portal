using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Client;

internal class Program
{
  static void Main(string[] args)
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .AddUserSecrets("b2b773e6-c1b2-4500-bcfa-a92c3e545a9c")
      .AddCommandLine(args)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(configuration);
    IServiceProvider serviceProvider = services.BuildServiceProvider();

    // TODO(fpion): implement
  }
}
