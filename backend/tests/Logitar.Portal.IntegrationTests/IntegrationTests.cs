using Bogus;
using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.SqlServer;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.Infrastructure.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

public abstract class IntegrationTests : IAsyncLifetime
{
  protected const string UsernameString = "portal";
  protected const string PasswordString = "P@s$W0rD";

  private readonly TestContext _context;
  private readonly DatabaseProvider _databaseProvider;

  protected Faker Faker { get; } = new();

  protected IServiceProvider ServiceProvider { get; }
  protected IActivityPipeline ActivityPipeline { get; }
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

    ServiceCollection services = new();
    services.AddSingleton(configuration);
    services.AddSingleton<IContextParametersResolver, TestContextParametersResolver>();
    services.AddScoped<IBaseUrl, TestBaseUrl>();
    services.AddScoped<TestContext>();

    string connectionString;
    _databaseProvider = configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (_databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        connectionString = configuration.GetValue<string>("POSTGRESQLCONNSTR_Portal")?.Replace("{Database}", GetType().Name) ?? string.Empty;
        services.AddLogitarPortalWithEntityFrameworkCorePostgreSQL(connectionString);
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = configuration.GetValue<string>("SQLCONNSTR_Portal")?.Replace("{Database}", GetType().Name) ?? string.Empty;
        services.AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(_databaseProvider);
    }

    ServiceProvider = services.BuildServiceProvider();
    _context = ServiceProvider.GetRequiredService<TestContext>();

    ActivityPipeline = ServiceProvider.GetRequiredService<IActivityPipeline>();
    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    IdentityContext = ServiceProvider.GetRequiredService<IdentityContext>();
    PortalContext = ServiceProvider.GetRequiredService<PortalContext>();

    Realm = new("tests", JwtSecret.Generate().Value)
    {
      Id = Guid.NewGuid(),
      DisplayName = "Tests",
      DefaultLocale = new LocaleModel(Faker.Locale),
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
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await publisher.Publish(new InitializeConfigurationCommand(UsernameString, PasswordString));

    IUserRepository userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
    UserAggregate userAggregate = Assert.Single(await userRepository.LoadAsync());
    ActorId actorId = new(userAggregate.Id.Value);
    userAggregate.FirstName = new PersonNameUnit(Faker.Person.FirstName);
    userAggregate.LastName = new PersonNameUnit(Faker.Person.LastName);
    userAggregate.Update(actorId);
    EmailUnit email = new(Faker.Person.Email, isVerified: false);
    userAggregate.SetEmail(email, actorId);
    await userRepository.SaveAsync(userAggregate);

    IUserQuerier userQuerier = ServiceProvider.GetRequiredService<IUserQuerier>();
    User? user = await userQuerier.ReadAsync(realm: null, userAggregate.Id.ToGuid());
    SetUser(user);
  }
  protected IDeleteBuilder CreateDeleteBuilder(TableId table) => _databaseProvider switch
  {
    DatabaseProvider.EntityFrameworkCorePostgreSQL => PostgresDeleteBuilder.From(table),
    DatabaseProvider.EntityFrameworkCoreSqlServer => SqlServerDeleteBuilder.From(table),
    _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
  };

  public virtual Task DisposeAsync() => Task.CompletedTask;

  protected void SetRealm() => _context.Realm = Realm;
  protected void SetApiKey(ApiKeyModel? apiKey)
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
