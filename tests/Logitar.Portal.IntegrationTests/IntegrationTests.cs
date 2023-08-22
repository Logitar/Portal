using Bogus;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.SqlServer;
using Logitar.Portal.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

public abstract class IntegrationTests
{
  protected const string PasswordString = "P@s$W0rD"; // TODO(fpion): Pizza7

  private readonly TestApplicationContext _applicationContext = new();
  private readonly IConfigurationService _configurationService;
  private readonly IPasswordService _passwordService;

  protected IntegrationTests()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddMemoryCache();
    services.AddSingleton<IApplicationContext>(_applicationContext);
    services.Configure<Pbkdf2Settings>(settings => settings.Iterations = 6);

    string connectionString;
    DatabaseProvider databaseProvider = configuration.GetValue<DatabaseProvider>("DatabaseProvider");
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        connectionString = (configuration.GetValue<string>("POSTGRESQLCONNSTR_Portal") ?? string.Empty)
          .Replace("{database}", GetType().Name);
        services.AddLogitarPortalWithEntityFrameworkCorePostgreSQL(connectionString);
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = (configuration.GetValue<string>("SQLCONNSTR_Portal") ?? string.Empty)
          .Replace("{database}", GetType().Name);
        services.AddLogitarPortalWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    ServiceProvider = services.BuildServiceProvider();

    _configurationService = ServiceProvider.GetRequiredService<IConfigurationService>();
    _passwordService = ServiceProvider.GetRequiredService<IPasswordService>();

    AggregateRepository = ServiceProvider.GetRequiredService<IAggregateRepository>();
    Mediator = ServiceProvider.GetRequiredService<IMediator>();
    SqlHelper = ServiceProvider.GetRequiredService<ISqlHelper>();
    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    IdentityContext = ServiceProvider.GetRequiredService<IdentityContext>();
    PortalContext = ServiceProvider.GetRequiredService<PortalContext>();
  }

  protected bool InitializeConfiguration { get; set; } = true;
  protected Actor Actor => _applicationContext.Actor;
  protected ActorId ActorId => _applicationContext.ActorId;
  protected ConfigurationAggregate? Configuration { get; private set; }
  protected UserAggregate? User { get; private set; }

  protected Faker Faker { get; } = new();

  protected IServiceProvider ServiceProvider { get; }

  protected IAggregateRepository AggregateRepository { get; }
  protected IMediator Mediator { get; }
  protected ISqlHelper SqlHelper { get; }

  protected EventContext EventContext { get; }
  protected IdentityContext IdentityContext { get; }
  protected PortalContext PortalContext { get; }

  public virtual async Task InitializeAsync()
  {
    await Mediator.Publish(new InitializeDatabaseCommand());

    TableId[] tables = new[]
    {
      PortalDb.Realms.Table,
      PortalDb.Actors.Table,
      Db.Sessions.Table,
      Db.Users.Table,
      Db.Events.Table
    };
    foreach (TableId table in tables)
    {
      ICommand delete = SqlHelper.DeleteFrom(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(delete.Text, delete.Parameters.ToArray());
    }

    if (InitializeConfiguration)
    {
      await InitializeConfigurationAsync();
    }
  }
  protected async Task InitializeConfigurationAsync()
  {
    InitializeConfigurationPayload payload = new()
    {
      Locale = Faker.Locale,
      User = new UserPayload
      {
        UniqueName = Faker.Person.UserName,
        Password = PasswordString,
        EmailAddress = Faker.Person.Email,
        FirstName = Faker.Person.FirstName,
        LastName = Faker.Person.LastName
      }
    };

    InitializeConfigurationResult result = await _configurationService.InitializeAsync(payload);
    User user = result.User;

    _applicationContext.Actor = new()
    {
      Id = user.Id,
      Type = ActorType.User,
      DisplayName = user.FullName ?? user.UniqueName,
      EmailAddress = user.Email?.Address,
      PictureUrl = user.Picture
    };

    Configuration = await AggregateRepository.LoadAsync<ConfigurationAggregate>(ConfigurationAggregate.UniqueId);
    Assert.NotNull(Configuration);

    User = await AggregateRepository.LoadAsync<UserAggregate>(new AggregateId(user.Id));
    Assert.NotNull(User);
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;

  protected virtual void AssertIsNear(DateTime? value, int seconds = 1)
  {
    if (value.HasValue)
    {
      Assert.True(DateTime.Now - value.Value < TimeSpan.FromSeconds(seconds));
    }
  }

  protected async Task CheckUserPasswordAsync(string userId, string passwordString)
  {
    UserEntity? user = await IdentityContext.Users.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == userId);
    Assert.NotNull(user);

    Assert.NotNull(user.Password);
    Password password = _passwordService.Decode(user.Password);
    Assert.True(password.IsMatch(passwordString));
  }
}
