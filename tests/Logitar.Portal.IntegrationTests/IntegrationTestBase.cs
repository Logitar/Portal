using Bogus;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.SqlServer;
using Logitar.Portal.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

public abstract class IntegrationTestBase
{
  protected IntegrationTestBase()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddMemoryCache();
    services.AddSingleton(configuration);
    services.AddSingleton<IApplicationContext, TestApplicationContext>();

    DatabaseProvider databaseProvider = configuration.GetValue<DatabaseProvider>("DatabaseProvider");
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        string connectionString = (configuration.GetValue<string>("SQLCONNSTR_Portal") ?? string.Empty)
          .Replace("{database}", GetType().Name);
        services.AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    ServiceProvider = services.BuildServiceProvider();

    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    IdentityContext = ServiceProvider.GetRequiredService<IdentityContext>();
    PortalContext = ServiceProvider.GetRequiredService<PortalContext>();
    Publisher = ServiceProvider.GetRequiredService<IPublisher>();
    SqlHelper = ServiceProvider.GetRequiredService<ISqlHelper>();

    Faker faker = new();
    Actor = new()
    {
      Id = AggregateId.NewId().Value,
      Type = ActorType.User,
      DisplayName = faker.Person.FullName,
      EmailAddress = faker.Person.Email,
      PictureUrl = faker.Person.Avatar
    };
  }

  protected Actor Actor { get; }
  protected ActorId ActorId => new(Actor.Id);
  protected Faker Faker { get; } = new();

  protected IServiceProvider ServiceProvider { get; }

  protected EventContext EventContext { get; }
  protected IdentityContext IdentityContext { get; }
  protected PortalContext PortalContext { get; }
  protected IPublisher Publisher { get; }
  protected ISqlHelper SqlHelper { get; }

  protected bool InitializeConfiguration { get; set; } = true;

  public virtual async Task InitializeAsync()
  {
    await Publisher.Publish(new InitializeDatabaseCommand());

    IEnumerable<TableId> tables = new[]
    {
      PortalDb.Realms.Table,
      PortalDb.Actors.Table,
      Db.Users.Table,
      Db.Events.Table
    };
    foreach (TableId table in tables)
    {
      ICommand command = SqlHelper.DeleteFrom(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    if (InitializeConfiguration)
    {
      await InitializeConfigurationAsync();
    }
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;

  protected async Task InitializeConfigurationAsync(string? locale = null, string? uniqueName = null,
    string? emailAddress = null, string? password = null, string? firstName = null, string? lastName = null)
  {
    InitializeConfigurationPayload payload = new()
    {
      Locale = locale ?? Faker.Locale,
      User = new UserPayload
      {
        UniqueName = uniqueName ?? Faker.Person.UserName,
        Password = password ?? "P@s$W0rD",
        EmailAddress = emailAddress ?? Faker.Person.Email,
        FirstName = firstName ?? Faker.Person.FirstName,
        LastName = lastName ?? Faker.Person.LastName
      }
    };

    IConfigurationService configurationService = ServiceProvider.GetRequiredService<IConfigurationService>();
    await configurationService.InitializeAsync(payload);
  }
}
