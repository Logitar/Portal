using Bogus;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
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
    TestContext context = new(count: 1 + 7 + 8 + 12 + 7 + 1 + 8 + 2 + 8 + 9 + 1);
    context.Start();

    if (!await InitializeConfigurationAsync(context, serviceProvider, cancellationToken)) // 1 test
    {
      context.End();
      return;
    }

    RealmClientTests realmTests = new(context, _faker, serviceProvider.GetRequiredService<IRealmService>()); // 7 tests
    if (!await realmTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    RoleClientTests roleTests = new(context, _faker, serviceProvider.GetRequiredService<IRoleService>()); // 8 tests
    if (!await roleTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    UserClientTests userTests = new(context, _faker, serviceProvider.GetRequiredService<IUserService>()); // 12 tests
    if (!await userTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    SessionClientTests sessionTests = new(context, _faker, serviceProvider.GetRequiredService<ISessionService>()); // 7 tests
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

    ApiKeyClientTests apiKeyTests = new(context, _faker, serviceProvider.GetRequiredService<IApiKeyService>()); // 8 tests
    if (!await apiKeyTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    TokenClientTests tokenTests = new(context, _faker, serviceProvider.GetRequiredService<ITokenService>()); // 2 tests
    if (!await tokenTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    DictionaryClientTests dictionaryTests = new(context, _faker, serviceProvider.GetRequiredService<IDictionaryService>()); // 8 tests
    if (!await dictionaryTests.ExecuteAsync(cancellationToken))
    {
      context.End();
      return;
    }

    SenderClientTests senderTests = new(context, _faker, serviceProvider.GetRequiredService<ISenderService>()); // 9 tests
    if (!await senderTests.ExecuteAsync(cancellationToken))
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
