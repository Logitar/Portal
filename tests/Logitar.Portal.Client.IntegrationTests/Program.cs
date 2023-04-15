using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Client;

internal class Program
{
  static async Task Main()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddUserSecrets("8d2d2156-5c9e-4d5d-828e-64f3c37d5829")
      .Build();

    PortalSettings settings = new()
    {
      BaseUrl = configuration.GetValue<string>("BaseUrl") ?? string.Empty
    };
    Credentials credentials = configuration.GetSection("Credentials").Get<Credentials>() ?? new();

    using HttpClient client = new();
    ConfigurationService configurationService = new(client, settings);
    TestContext context = new(count: 48);
    context.Start();

    ConfigurationServiceTests configurationTests = new(configurationService);
    await configurationTests.ExecuteAsync(context, credentials); // 1 test

    if (context.HasFailed)
    {
      context.End();
      return;
    }

    settings.BasicAuthentication = credentials;

    IServiceProvider serviceProvider = new ServiceCollection()
      .AddHttpClient()
      .AddLogitarPortalClient(settings)
      .AddSingleton(configuration)
      .AddSingleton(context)
      .AddTransient<DictionaryServiceTests>()
      .AddTransient<MessageServiceTests>()
      .AddTransient<RealmServiceTests>()
      .AddTransient<SenderServiceTests>()
      .AddTransient<SessionServiceTests>()
      .AddTransient<TemplateServiceTests>()
      .AddTransient<TokenServiceTests>()
      .AddTransient<UserServiceTests>()
      .BuildServiceProvider();

    RealmServiceTests realmTests = serviceProvider.GetRequiredService<RealmServiceTests>();
    Realm? realm = await realmTests.ExecuteAsync(); // 5 tests
    if (realm == null)
    {
      context.End();
      return;
    }
    context.Realm = realm;

    DictionaryServiceTests dictionaryTests = serviceProvider.GetRequiredService<DictionaryServiceTests>(); // 5 tests
    if (await dictionaryTests.ExecuteAsync() == null)
    {
      context.End();
      return;
    }

    SenderServiceTests senderTests = serviceProvider.GetRequiredService<SenderServiceTests>(); // 6 tests
    if (await senderTests.ExecuteAsync() == null)
    {
      context.End();
      return;
    }

    TemplateServiceTests templateTests = serviceProvider.GetRequiredService<TemplateServiceTests>(); // 5 tests
    if (await templateTests.ExecuteAsync() == null)
    {
      context.End();
      return;
    }

    TokenServiceTests tokenTests = serviceProvider.GetRequiredService<TokenServiceTests>(); // 3 tests
    if (!await tokenTests.ExecuteAsync())
    {
      context.End();
      return;
    }

    UserServiceTests userTests = serviceProvider.GetRequiredService<UserServiceTests>(); // 14 tests
    User? user = await userTests.ExecuteAsync(credentials);
    if (user == null)
    {
      context.End();
      return;
    }

    SessionServiceTests sessionTests = serviceProvider.GetRequiredService<SessionServiceTests>(); // 6 tests
    if (await sessionTests.ExecuteAsync(credentials) == null)
    {
      context.End();
      return;
    }

    MessageServiceTests messageTests = serviceProvider.GetRequiredService<MessageServiceTests>(); // 6 tests
    _ = await messageTests.ExecuteAsync(user);

    context.End();
  }
}
