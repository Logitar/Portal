using Logitar.Portal.Contracts.Messages;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Logitar.Portal.MassTransit;

internal class Program
{
  static readonly JsonSerializerOptions _serializerOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  static async Task Main(string[] args)
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .AddUserSecrets("2575b63b-34ec-4ba4-b347-1c8829f50bb3")
      .AddCommandLine(args)
      .Build();

    IServiceProvider serviceProvider = new ServiceCollection()
      .AddSingleton(configuration)
      .BuildServiceProvider();

    Console.Write("Enter your realm ID or unique slug (or leave blank): ");
    string? realm = Console.ReadLine()?.CleanTrim();

    Console.Write("Enter your user ID: ");
    Guid userId = Guid.Parse(Console.ReadLine()?.Trim() ?? string.Empty);

    Console.Write("Enter your template ID or unique key: ");
    string template = Console.ReadLine()?.Trim() ?? string.Empty;

    Console.Write("Enter your sender ID (or leave blank): ");
    string? senderIdInput = Console.ReadLine()?.CleanTrim();

    Console.Write("Ignore user locale? (true/false)");
    string ignoreUserLocale = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

    Console.Write("Enter your locale code (or leave blank): ");
    string? locale = Console.ReadLine()?.Trim();

    Console.Write("Paste your variables as JSON '[{key:string,value:string}]' (or leave blank): ");
    string? variablesInput = Console.ReadLine()?.Trim();

    SendMessagePayload payload = new(template)
    {
      IgnoreUserLocale = ignoreUserLocale == "true",
      Locale = locale,
      IsDemo = true
    };
    if (Guid.TryParse(senderIdInput, out Guid senderId))
    {
      payload.SenderId = senderId;
    }
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      UserId = userId
    });
    if (variablesInput != null)
    {
      IEnumerable<Variable>? variables = JsonSerializer.Deserialize<IEnumerable<Variable>>(variablesInput, _serializerOptions);
      if (variables != null)
      {
        payload.Variables.AddRange(variables);
      }
    }
    SendMessageCommand command = new(payload);

    IBus bus = serviceProvider.GetRequiredService<IBus>();
    Guid correlationId = NewId.NextGuid();
    await bus.Publish(command, c =>
    {
      c.CorrelationId = correlationId;

      if (realm != null)
      {
        c.Headers.Set(Contracts.Constants.Headers.Realm, realm);
      }
      c.Headers.Set(Contracts.Constants.Headers.User, userId.ToString());
    });
  }
}
