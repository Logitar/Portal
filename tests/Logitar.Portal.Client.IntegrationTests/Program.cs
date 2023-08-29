﻿using Bogus;
using Logitar.Portal.Contracts.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Client.IntegrationTests;

internal class Program
{
  private const string PasswordString = "Test123!";
  private static readonly Faker _faker = new();

  static async Task Main(string[] args)
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddCommandLine(args)
      .AddJsonFile("appsettings.json")
      .Build();

    PortalSettings settings = new()
    {
      BaseUrl = configuration.GetValue<string>("BaseUrl") ?? string.Empty,
      BasicAuthentication = new Credentials
      {
        Username = _faker.Person.UserName,
        Password = PasswordString
      }
    };

    IServiceProvider serviceProvider = new ServiceCollection()
      .AddSingleton(configuration)
      .AddLogitarPortalClient(settings)
      .BuildServiceProvider();

    Console.Write("Press a key to start the integration tests.");
    Console.ReadKey(intercept: true);
    Console.WriteLine();
    Console.WriteLine();

    CancellationToken cancellationToken = default;
    TestContext context = new(count: 1);
    context.Start();

    await InitializeConfigurationAsync(context, serviceProvider, cancellationToken);
    if (context.HasFailed)
    {
      context.End();
      return;
    }

    context.End();
  }

  private static async Task InitializeConfigurationAsync(TestContext context, IServiceProvider serviceProvider, CancellationToken cancellationToken)
  {
    string name = nameof(InitializeConfigurationAsync);

    try
    {
      HttpClient client = serviceProvider.GetRequiredService<HttpClient>();
      PortalSettings settings = serviceProvider.GetRequiredService<PortalSettings>();
      if (settings.BasicAuthentication == null)
      {
        throw new InvalidOperationException($"The 'Portal.{nameof(settings.BasicAuthentication)}' settings are required.");
      }

      InitializeConfigurationPayload payload = new()
      {
        Locale = _faker.Locale,
        User = new UserPayload
        {
          UniqueName = settings.BasicAuthentication.Username,
          Password = settings.BasicAuthentication.Password,
          EmailAddress = _faker.Person.Email,
          FirstName = _faker.Person.FirstName,
          LastName = _faker.Person.LastName
        }
      };
      ConfigurationClient service = new(client, settings);
      _ = await service.InitializeAsync(payload, cancellationToken);

      context.Succeed(name);
    }
    catch (Exception exception)
    {
      context.Fail(name, exception);
    }
  }
}