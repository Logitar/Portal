using Bogus;
using Logitar.Data;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.SqlServer;
using Logitar.Portal.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Services;

[Trait("Category", "Integration")]
public class ConfigurationServiceTests : IAsyncLifetime
{
  private readonly Actor _actor = new();
  private readonly Faker _faker = new();

  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IConfigurationService _configurationService;
  private readonly IMediator _mediator;
  private readonly PortalContext _portalContext;
  private readonly ISqlHelper _sqlHelper;
  private readonly IUserRepository _userRepository;
  private readonly IOptions<UserSettings> _userSettings;

  public ConfigurationServiceTests()
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

    IServiceProvider serviceProvider = services.BuildServiceProvider();

    _cacheService = serviceProvider.GetRequiredService<ICacheService>();
    _configurationRepository = serviceProvider.GetRequiredService<IConfigurationRepository>();
    _configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
    _mediator = serviceProvider.GetRequiredService<IMediator>();
    _portalContext = serviceProvider.GetRequiredService<PortalContext>();
    _sqlHelper = serviceProvider.GetRequiredService<ISqlHelper>();
    _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
    _userSettings = serviceProvider.GetRequiredService<IOptions<UserSettings>>();
  }

  public async Task InitializeAsync()
  {
    await _mediator.Publish(new InitializeDatabaseCommand());

    IEnumerable<TableId> tables = new[]
    {
      PortalDb.Realms.Table,
      PortalDb.Actors.Table,
      Db.Users.Table,
      Db.Events.Table
    };
    foreach (TableId table in tables)
    {
      ICommand command = _sqlHelper.DeleteFrom(table).Build();
      await _portalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }
  public Task DisposeAsync() => Task.CompletedTask;

  [Fact(DisplayName = "InitializeAsync: it should initialize the configuration.")]
  public async Task InitializeAsync_it_should_initialize_the_configuration()
  {
    InitializeConfigurationPayload payload = new()
    {
      Locale = $" {_faker.Locale} ",
      User = new UserPayload
      {
        UniqueName = $" {_faker.Person.UserName} ",
        Password = "Pizza7",
        EmailAddress = $" {_faker.Person.Email} ",
        FirstName = $" {_faker.Person.FirstName} ",
        LastName = $" {_faker.Person.LastName} "
      }
    };

    await _configurationService.InitializeAsync(payload);

    UserAggregate? user = await _userRepository.LoadAsync(tenantId: null, payload.User.UniqueName);
    Assert.NotNull(user);
    Assert.Equal(user.Id.Value, user.CreatedBy.Value);
    Assert.Equal(user.Id.Value, user.UpdatedBy.Value);
    Assert.Equal(payload.User.UniqueName.Trim(), user.UniqueName);
    Assert.Equal(payload.User.EmailAddress.Trim(), user.Email?.Address);
    Assert.Equal(payload.User.FirstName.Trim(), user.FirstName);
    Assert.Equal(payload.User.LastName.Trim(), user.LastName);
    Assert.Equal(payload.Locale.Trim(), user.Locale?.Name);
    Assert.True(user.HasPassword);
    _ = user.SignIn(_userSettings.Value, payload.User.Password);

    ConfigurationAggregate? configuration = _cacheService.Configuration;
    Assert.NotNull(configuration);
    Assert.Equal(user.Id.Value, configuration.CreatedBy.Value);
    Assert.Equal(user.Id.Value, configuration.UpdatedBy.Value);
    Assert.Equal(configuration.CreatedOn, configuration.UpdatedOn);
    Assert.Equal(1, configuration.Version);
    Assert.Equal(payload.Locale.Trim(), configuration.DefaultLocale.Name);
    Assert.Equal(32, configuration.Secret.Length);
    Assert.Equal(new ReadOnlyUniqueNameSettings(), configuration.UniqueNameSettings);
    Assert.Equal(new ReadOnlyPasswordSettings(), configuration.PasswordSettings);
    Assert.Equal(new ReadOnlyLoggingSettings(), configuration.LoggingSettings);
  }

  [Fact(DisplayName = "ReadAsync: it should read the configuration.")]
  public async Task ReadAsync_it_should_read_the_configuration()
  {
    await InitializeConfigurationAsync();

    Configuration configuration = await _configurationService.ReadAsync();
    Assert.NotNull(configuration);
    Assert.Equal(_faker.Locale, configuration.DefaultLocale);
    Assert.Equal(32, configuration.Secret.Length);
    Assert.Equal(new Contracts.UniqueNameSettings(), configuration.UniqueNameSettings);
    Assert.Equal(new Contracts.PasswordSettings(), configuration.PasswordSettings);
    Assert.Equal(new LoggingSettings(), configuration.LoggingSettings);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the configuration.")]
  public async Task ReplaceAsync_it_should_replace_the_configuration()
  {
    await InitializeConfigurationAsync();

    ConfigurationAggregate? aggregate = await _configurationRepository.LoadAsync();
    Assert.NotNull(aggregate);

    ReplaceConfigurationPayload payload = new()
    {
      DefaultLocale = "fr-CA",
      Secret = string.Empty,
      UniqueNameSettings = new Contracts.UniqueNameSettings
      {
        AllowedCharacters = aggregate.UniqueNameSettings.AllowedCharacters
      },
      PasswordSettings = new Contracts.PasswordSettings
      {
        RequiredLength = 10,
        RequiredUniqueChars = 6,
        RequireNonAlphanumeric = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = true
      },
      LoggingSettings = new LoggingSettings
      {
        Extent = LoggingExtent.Full,
        OnlyErrors = true
      }
    };
    Configuration configuration = await _configurationService.ReplaceAsync(payload, aggregate.Version);
    Assert.NotNull(configuration);
    Assert.Equal(_actor, configuration.UpdatedBy);
    Assert.True(configuration.Version > aggregate.Version);
    Assert.Equal(payload.DefaultLocale, configuration.DefaultLocale);
    Assert.NotEqual(aggregate.Secret, configuration.Secret);
    Assert.Equal(aggregate.UniqueNameSettings, configuration.UniqueNameSettings.ToReadOnlyUniqueNameSettings());
    Assert.Equal(payload.PasswordSettings, configuration.PasswordSettings);
    Assert.Equal(payload.LoggingSettings, configuration.LoggingSettings);
  }

  [Fact(DisplayName = "UpdateAsync: it should update the configuration.")]
  public async Task UpdateAsync_it_should_update_the_configuration()
  {
    await InitializeConfigurationAsync();

    ConfigurationAggregate? aggregate = await _configurationRepository.LoadAsync();
    Assert.NotNull(aggregate);

    UpdateConfigurationPayload payload = new()
    {
      DefaultLocale = "fr-CA"
    };
    Configuration configuration = await _configurationService.UpdateAsync(payload);
    Assert.NotNull(configuration);
    Assert.Equal(_actor, configuration.UpdatedBy);
    Assert.True(configuration.Version > aggregate.Version);
    Assert.Equal(payload.DefaultLocale, configuration.DefaultLocale);
    Assert.Equal(aggregate.Secret, configuration.Secret);
    Assert.Equal(aggregate.UniqueNameSettings, configuration.UniqueNameSettings.ToReadOnlyUniqueNameSettings());
    Assert.Equal(aggregate.PasswordSettings, configuration.PasswordSettings.ToReadOnlyPasswordSettings());
    Assert.Equal(aggregate.LoggingSettings, configuration.LoggingSettings.ToReadOnlyLoggingSettings());
  }

  private async Task InitializeConfigurationAsync(CancellationToken cancellationToken = default)
  {
    InitializeConfigurationPayload payload = new()
    {
      Locale = _faker.Locale,
      User = new UserPayload
      {
        UniqueName = _faker.Person.UserName,
        Password = "P@s$W0rD",
        EmailAddress = _faker.Person.Email,
        FirstName = _faker.Person.FirstName,
        LastName = _faker.Person.LastName
      }
    };
    await _configurationService.InitializeAsync(payload, cancellationToken);
  }
}
