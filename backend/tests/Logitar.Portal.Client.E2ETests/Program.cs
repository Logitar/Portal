﻿using Logitar.Portal.ApiKeys;
using Logitar.Portal.Configurations;
using Logitar.Portal.Dictionaries;
using Logitar.Portal.OneTimePasswords;
using Logitar.Portal.Realms;
using Logitar.Portal.Roles;
using Logitar.Portal.Senders;
using Logitar.Portal.Sessions;
using Logitar.Portal.Templates;
using Logitar.Portal.Tokens;
using Logitar.Portal.Users;
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

    Console.Write("Press a key to start the end-to-end tests.");
    Console.ReadKey(intercept: true);
    Console.WriteLine();
    Console.WriteLine();

    TestContext context = TestContext.Start(count: 1 + 3 + 1 + 6 + 6 + 7 + 11 + 6 + 4 + 2 + 6 + 8 + 6);

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

    CreateApiKeyTests createApiKeyTests = serviceProvider.GetRequiredService<CreateApiKeyTests>();
    if (!await createApiKeyTests.ExecuteAsync(context)) // 1 test
    {
      context.End();
      return;
    }

    RealmClientTests realmTests = serviceProvider.GetRequiredService<RealmClientTests>();
    if (!await realmTests.ExecuteAsync(context)) // 6 tests
    {
      context.End();
      return;
    }

    RoleClientTests roleTests = serviceProvider.GetRequiredService<RoleClientTests>();
    if (!await roleTests.ExecuteAsync(context)) // 6 tests
    {
      context.End();
      return;
    }

    ApiKeyClientTests apiKeyTests = serviceProvider.GetRequiredService<ApiKeyClientTests>();
    if (!await apiKeyTests.ExecuteAsync(context)) // 7 tests
    {
      context.End();
      return;
    }

    UserClientTests userTests = serviceProvider.GetRequiredService<UserClientTests>();
    if (!await userTests.ExecuteAsync(context)) // 11 tests
    {
      context.End();
      return;
    }

    SessionClientTests sessionTests = serviceProvider.GetRequiredService<SessionClientTests>();
    if (!await sessionTests.ExecuteAsync(context)) // 6 tests
    {
      context.End();
      return;
    }

    OneTimePasswordClientTests oneTimePasswordTests = serviceProvider.GetRequiredService<OneTimePasswordClientTests>();
    if (!await oneTimePasswordTests.ExecuteAsync(context)) // 4 tests
    {
      context.End();
      return;
    }

    TokenClientTests tokenTests = serviceProvider.GetRequiredService<TokenClientTests>();
    if (!await tokenTests.ExecuteAsync(context)) // 2 tests
    {
      context.End();
      return;
    }

    DictionaryClientTests dictionaryTests = serviceProvider.GetRequiredService<DictionaryClientTests>();
    if (!await dictionaryTests.ExecuteAsync(context)) // 6 tests
    {
      context.End();
      return;
    }

    SenderClientTests senderTests = serviceProvider.GetRequiredService<SenderClientTests>();
    if (!await senderTests.ExecuteAsync(context)) // 8 tests
    {
      context.End();
      return;
    }

    TemplateClientTests templateTests = serviceProvider.GetRequiredService<TemplateClientTests>();
    if (!await templateTests.ExecuteAsync(context)) // 6 tests
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
