using Bogus;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class ConfigurationServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IConfigurationService _configurationService;

  public ConfigurationServiceTests() : base()
  {
    _configurationService = ServiceProvider.GetRequiredService<IConfigurationService>();

    InitializeConfiguration = false;
  }

  [Fact(DisplayName = "InitializeAsync: it should initialize the configuration.")]
  public async Task InitializeAsync_it_should_initialize_the_configuration()
  {
    InitializeConfigurationPayload payload = new()
    {
      Locale = $" {Faker.Locale} ",
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
    Configuration configuration = result.Configuration;
    User user = result.User;
    Assert.NotNull(user.Email);
    Assert.NotNull(user.FullName);

    Actor actor = new()
    {
      Id = result.User.Id,
      Type = ActorType.User,
      DisplayName = user.FullName,
      EmailAddress = user.Email.Address
    };

    Assert.Equal(ConfigurationAggregate.UniqueId.Value, configuration.Id);
    AssertIsNear(configuration.CreatedOn);
    Assert.Equal(configuration.CreatedOn, configuration.UpdatedOn);
    Assert.Equal(actor, configuration.CreatedBy);
    Assert.Equal(actor, configuration.UpdatedBy);
    Assert.Equal(1, configuration.Version);
    Assert.Equal(payload.Locale.Trim(), configuration.DefaultLocale);
    Assert.Equal(32, configuration.Secret.Length);
    Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+", configuration.UniqueNameSettings.AllowedCharacters);
    Assert.Equal(6, configuration.PasswordSettings.RequiredLength);
    Assert.Equal(1, configuration.PasswordSettings.RequiredUniqueChars);
    Assert.False(configuration.PasswordSettings.RequireNonAlphanumeric);
    Assert.True(configuration.PasswordSettings.RequireLowercase);
    Assert.True(configuration.PasswordSettings.RequireUppercase);
    Assert.True(configuration.PasswordSettings.RequireDigit);
    Assert.Equal(LoggingExtent.ActivityOnly, configuration.LoggingSettings.Extent);
    Assert.False(configuration.LoggingSettings.OnlyErrors);

    Assert.Equal(actor, user.CreatedBy);
    AssertIsNear(user.CreatedOn);
    Assert.Equal(actor, user.UpdatedBy);
    AssertIsNear(user.UpdatedOn);
    Assert.True(user.Version >= 1);
    Assert.Equal(payload.User.UniqueName, user.UniqueName);
    Assert.True(user.HasPassword);
    Assert.Equal(actor, user.PasswordChangedBy);
    AssertIsNear(user.PasswordChangedOn);
    Assert.False(user.IsDisabled);
    Assert.Equal(payload.User.EmailAddress, user.Email.Address);
    Assert.False(user.IsConfirmed);
    Assert.Equal(payload.User.FirstName, user.FirstName);
    Assert.Equal(payload.User.LastName, user.LastName);
    Assert.Equal(payload.Locale.Trim(), user.Locale);
    Assert.Null(user.Realm);

    ActorEntity actorEntity = await PortalContext.Actors.AsNoTracking().SingleAsync(a => a.Id == user.Id);
    Assert.Equal(actor, actorEntity.ToActor());

    await CheckUserPasswordAsync(user.Id, payload.User.Password);
  }

  [Fact(DisplayName = "InitializeAsync: it should throw ConfigurationAlreadyInitializedException when configuration is initialized.")]
  public async Task InitializeAsync_it_should_throw_ConfigurationAlreadyInitializedException_when_configuration_is_initialized()
  {
    await InitializeConfigurationAsync();

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

    await Assert.ThrowsAsync<ConfigurationAlreadyInitializedException>(
      async () => await _configurationService.InitializeAsync(payload)
    );
  }

  [Fact(DisplayName = "InitializeAsync: it should throw InvalidLocaleException when locale is empty.")]
  public async Task InitializeAsync_it_should_throw_InvalidLocaleException_when_locale_is_empty()
  {
    InitializeConfigurationPayload payload = new()
    {
      Locale = "  ",
      User = new UserPayload
      {
        UniqueName = Faker.Person.UserName,
        Password = PasswordString,
        EmailAddress = Faker.Person.Email,
        FirstName = Faker.Person.FirstName,
        LastName = Faker.Person.LastName
      }
    };

    var exception = await Assert.ThrowsAsync<InvalidLocaleException>(
      async () => await _configurationService.InitializeAsync(payload)
    );
    Assert.Equal(payload.Locale, exception.Code);
    Assert.Equal(nameof(payload.Locale), exception.PropertyName);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when configuration has not been initialized.")]
  public async Task ReadAsync_it_should_return_null_when_configuration_has_not_been_initialized()
  {
    Assert.Null(await _configurationService.ReadAsync());
  }

  [Fact(DisplayName = "ReadAsync: it should return the initialized configuration.")]
  public async Task ReadAsync_it_should_return_the_initialized_configuration()
  {
    await InitializeConfigurationAsync();

    Configuration? configuration = await _configurationService.ReadAsync();
    Assert.NotNull(configuration);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw InvalidOperationException when configuration has not been initialized.")]
  public async Task UpdateAsync_it_should_throw_InvalidOperationException_when_configuration_has_not_been_initialized()
  {
    UpdateConfigurationPayload payload = new();

    var exception = await Assert.ThrowsAsync<InvalidOperationException>(
      async () => await _configurationService.UpdateAsync(payload)
    );
    Assert.StartsWith("The configuration could not be found.", exception.Message);
  }

  [Fact(DisplayName = "UpdateAsync: it should update the initialized configuration.")]
  public async Task UpdateAsync_it_should_update_the_initialized_configuration()
  {
    await InitializeConfigurationAsync();

    string oldSecret = string.Empty;

    UpdateConfigurationPayload payload = new()
    {
      DefaultLocale = " en-CA ",
      Secret = string.Empty,
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 8,
        RequiredUniqueChars = 8,
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

    Configuration configuration = await _configurationService.UpdateAsync(payload);
    Assert.Equal(Actor, configuration.UpdatedBy);
    AssertIsNear(configuration.UpdatedOn);
    Assert.True(configuration.Version > 1);
    Assert.Equal(payload.DefaultLocale.Trim(), configuration.DefaultLocale);
    Assert.NotEqual(oldSecret, configuration.Secret);
    Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+", configuration.UniqueNameSettings.AllowedCharacters);
    Assert.Equal(payload.PasswordSettings, configuration.PasswordSettings);
    Assert.Equal(payload.LoggingSettings, configuration.LoggingSettings);
  }
}
