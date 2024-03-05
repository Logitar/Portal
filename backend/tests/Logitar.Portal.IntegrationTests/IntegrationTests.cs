using Bogus;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.SqlServer;
using Logitar.Portal.Infrastructure.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

public abstract class IntegrationTests : IAsyncLifetime
{
  protected const string PasswordString = "P@s$W0rD";

  private readonly TestContext _context;

  protected Faker Faker { get; } = new();

  protected IServiceProvider ServiceProvider { get; }
  protected IMediator Mediator { get; }
  protected EventContext EventContext { get; }
  protected IdentityContext IdentityContext { get; }
  protected PortalContext PortalContext { get; }

  protected Realm Realm { get; }
  protected TenantId TenantId => Realm.GetTenantId();

  protected IntegrationTests()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .AddUserSecrets("f5e49206-87cc-4c7a-8962-e79ebd5c850c")
      .Build();

    string connectionString = configuration.GetValue<string>("SQLCONNSTR_Portal")
      ?.Replace("{Database}", GetType().Name) ?? string.Empty;

    ServiceProvider = new ServiceCollection()
      .AddSingleton(configuration)
      .AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString)
      .AddScoped<IBaseUrl, TestBaseUrl>()
      .AddScoped<TestContext>()
      .AddTransient(typeof(IPipelineBehavior<,>), typeof(TestContextualizationBehavior<,>))
      .BuildServiceProvider();

    _context = ServiceProvider.GetRequiredService<TestContext>();

    Mediator = ServiceProvider.GetRequiredService<IMediator>();
    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    IdentityContext = ServiceProvider.GetRequiredService<IdentityContext>();
    PortalContext = ServiceProvider.GetRequiredService<PortalContext>();

    Realm = new("tests", JwtSecretUnit.Generate().Value)
    {
      Id = Guid.NewGuid(),
      DisplayName = "Tests",
      DefaultLocale = new Locale(Faker.Locale),
      Url = $"https://www.{Faker.Internet.DomainName()}",
      RequireUniqueEmail = true
    };
  }

  public virtual async Task InitializeAsync()
  {
    IPublisher publisher = ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    TableId[] tables = [EventDb.Events.Table, IdentityDb.Actors.Table, IdentityDb.CustomAttributes.Table, IdentityDb.Sessions.Table, IdentityDb.Users.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await publisher.Publish(new InitializeConfigurationCommand());

    IUserQuerier userQuerier = ServiceProvider.GetRequiredService<IUserQuerier>();
    SearchResults<User> users = await userQuerier.SearchAsync(realm: null, new SearchUsersPayload());
    SetUser(users.Items.Single());
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;

  protected void SetRealm() => _context.Realm = Realm;
  protected void SetApiKey(ApiKey? apiKey)
  {
    _context.ApiKey = apiKey;
  }
  protected void SetUser(User? user)
  {
    _context.User = user;
    _context.Session = null;
  }
  protected void SetSession(Session? session)
  {
    _context.User = session?.User;
    _context.Session = session;
  }
}
