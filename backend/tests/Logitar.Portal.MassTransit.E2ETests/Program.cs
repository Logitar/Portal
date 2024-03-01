using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.MassTransit.Caching;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.MassTransit;

internal class Program
{
  static async Task Main(string[] args)
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .AddUserSecrets("2575b63b-34ec-4ba4-b347-1c8829f50bb3")
      .AddCommandLine(args)
      .Build();
    IServiceProvider serviceProvider = CreateServiceProvider(configuration);

    Console.Write("Press a key to start the end-to-end test sequence.");
    Console.ReadKey(intercept: true);
    Console.WriteLine();
    Console.WriteLine();

    string? realm = configuration.GetValue<string>("Realm");
    string? user = configuration.GetValue<string>("User");
    SendMessagePayload payload = configuration.GetSection("SendMessagePayload").Get<SendMessagePayload>() ?? new();
    payload.IsDemo = true;

    SendMessageCommand command = new(payload);
    IBus bus = serviceProvider.GetRequiredService<IBus>();
    Guid correlationId = NewId.NextGuid();
    await bus.Publish(command, c =>
    {
      c.CorrelationId = correlationId;

      if (!string.IsNullOrWhiteSpace(realm))
      {
        c.Headers.Set(Contracts.Constants.Headers.Realm, realm.Trim());
      }
      if (!string.IsNullOrWhiteSpace(user))
      {
        c.Headers.Set(Contracts.Constants.Headers.User, user.Trim());
      }
    });
    Console.WriteLine("{0} sent for correlation ID '{1}'.", nameof(SendMessageCommand), correlationId);

    ICacheService cacheService = serviceProvider.GetRequiredService<ICacheService>();
    int totalDelay = 0;
    for (int i = 0; i < 10; i++)
    {
      int millisecondsDelay = (int)Math.Pow(2, i) * 100;
      await Task.Delay(millisecondsDelay);
      totalDelay += millisecondsDelay;

      SentMessages? sentMessages = cacheService.GetSentMessages(correlationId);
      if (sentMessages == null)
      {
        Console.WriteLine("No sent messages yet.");
      }
      else
      {
        Console.WriteLine("SUCCESS: {0} message(s) sent after {1}ms.", sentMessages.Ids.Count, totalDelay);
        return;
      }
    }

    Console.WriteLine("ERROR: no sent messages after {0}ms.", totalDelay);
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
