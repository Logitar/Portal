using Bogus;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Services;

[Trait("Category", "Integration")]
public class ConfigurationServiceTests : IntegrationTestBase, IAsyncLifetime
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IConfigurationService _configurationService;
  private readonly IUserRepository _userRepository;
  private readonly IOptions<UserSettings> _userSettings;

  public ConfigurationServiceTests()
  {
    InitializeConfiguration = false;

    _cacheService = ServiceProvider.GetRequiredService<ICacheService>();
    _configurationRepository = ServiceProvider.GetRequiredService<IConfigurationRepository>();
    _configurationService = ServiceProvider.GetRequiredService<IConfigurationService>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
    _userSettings = ServiceProvider.GetRequiredService<IOptions<UserSettings>>();
  }

  [Fact(DisplayName = "InitializeAsync: it should initialize the configuration.")]
  public async Task InitializeAsync_it_should_initialize_the_configuration()
  {
    InitializeConfigurationPayload payload = new()
    {
      Locale = $" {Faker.Locale} ",
      User = new UserPayload
      {
        UniqueName = $" {Faker.Person.UserName} ",
        Password = "Pizza7",
        EmailAddress = $" {Faker.Person.Email} ",
        FirstName = $" {Faker.Person.FirstName} ",
        LastName = $" {Faker.Person.LastName} "
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

  [Fact(DisplayName = "InitializeAsync: it should throw ConfigurationAlreadyInitializedException when configuration has already been initialized.")]
  public async Task InitializeAsync_it_should_throw_ConfigurationAlreadyInitializedException_when_configuration_has_already_been_initialized()
  {
    await InitializeConfigurationAsync();

    InitializeConfigurationPayload payload = new();
    await Assert.ThrowsAsync<ConfigurationAlreadyInitializedException>(async () => await _configurationService.InitializeAsync(payload));
  }

  [Fact(DisplayName = "ReadAsync: it should read the configuration.")]
  public async Task ReadAsync_it_should_read_the_configuration()
  {
    await InitializeConfigurationAsync();

    Configuration configuration = await _configurationService.ReadAsync();
    Assert.NotNull(configuration);
    Assert.Equal(Faker.Locale, configuration.DefaultLocale);
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
    Assert.Equal(new Actor()/*Actor*/, configuration.UpdatedBy); // TODO(fpion): resolve actor
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
    Assert.Equal(new Actor()/*Actor*/, configuration.UpdatedBy); // TODO(fpion): resolve actor
    Assert.True(configuration.Version > aggregate.Version);
    Assert.Equal(payload.DefaultLocale, configuration.DefaultLocale);
    Assert.Equal(aggregate.Secret, configuration.Secret);
    Assert.Equal(aggregate.UniqueNameSettings, configuration.UniqueNameSettings.ToReadOnlyUniqueNameSettings());
    Assert.Equal(aggregate.PasswordSettings, configuration.PasswordSettings.ToReadOnlyPasswordSettings());
    Assert.Equal(aggregate.LoggingSettings, configuration.LoggingSettings.ToReadOnlyLoggingSettings());
  }
}
