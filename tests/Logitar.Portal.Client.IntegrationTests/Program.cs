using Bogus;
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
      .AddJsonFile("appsettings.json")
      .AddUserSecrets("f025312d-905b-40e5-8eee-e06d4afe6b51")
      .AddCommandLine(args)
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

    TestContext context = new(count: 1 + 7 + 8 + 12 + 7 + 1 + 8 + 2 + 8 + 9 + 8 + 4 + 1);

    IServiceProvider serviceProvider = new ServiceCollection()
      .AddSingleton(configuration)
      .AddSingleton(context)
      .AddSingleton(_faker)
      .AddLogitarPortalClient(settings)
      .AddTransient<ApiKeyClientTests>()
      .AddTransient<DictionaryClientTests>()
      .AddTransient<MessageClientTests>()
      .AddTransient<RealmClientTests>()
      .AddTransient<RoleClientTests>()
      .AddTransient<SenderClientTests>()
      .AddTransient<SessionClientTests>()
      .AddTransient<TemplateClientTests>()
      .AddTransient<TokenClientTests>()
      .AddTransient<UserClientTests>()
      .BuildServiceProvider();

    Console.Write("Press a key to start the integration tests.");
    Console.ReadKey(intercept: true);
    Console.WriteLine();
    Console.WriteLine();

    CancellationToken cancellationToken = default;
    context.Start();

    if (!await InitializeConfigurationAsync(context, serviceProvider, cancellationToken)) // 1 test
    {
      context.End();
      return;
    }

    RealmClientTests realmTests = serviceProvider.GetRequiredService<RealmClientTests>(); // 7 tests
    if (!await realmTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    RoleClientTests roleTests = serviceProvider.GetRequiredService<RoleClientTests>(); // 8 tests
    if (!await roleTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    UserClientTests userTests = serviceProvider.GetRequiredService<UserClientTests>(); // 12 tests
    if (!await userTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    SessionClientTests sessionTests = serviceProvider.GetRequiredService<SessionClientTests>(); // 7 tests
    if (!await sessionTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    if (!await userTests.SignOutAsync(cancellationToken)) // 1 test
    {
      context.End();
      return;
    }

    ApiKeyClientTests apiKeyTests = serviceProvider.GetRequiredService<ApiKeyClientTests>(); // 8 tests
    if (!await apiKeyTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    TokenClientTests tokenTests = serviceProvider.GetRequiredService<TokenClientTests>(); // 2 tests
    if (!await tokenTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    DictionaryClientTests dictionaryTests = serviceProvider.GetRequiredService<DictionaryClientTests>(); // 8 tests
    if (!await dictionaryTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    SenderClientTests senderTests = serviceProvider.GetRequiredService<SenderClientTests>(); // 9 tests
    if (!await senderTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    TemplateClientTests templateTests = serviceProvider.GetRequiredService<TemplateClientTests>(); // 8 tests
    if (!await templateTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    MessageClientTests messageTests = serviceProvider.GetRequiredService<MessageClientTests>(); // 4 tests
    if (!await messageTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    await realmTests.DeleteAsync(cancellationToken); // 1 test
    context.End();
  }

  private static async Task<bool> InitializeConfigurationAsync(TestContext context, IServiceProvider serviceProvider, CancellationToken cancellationToken)
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

    return !context.HasFailed;
  }
}
