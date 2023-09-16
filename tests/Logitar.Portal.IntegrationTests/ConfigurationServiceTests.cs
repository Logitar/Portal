using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Configurations;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class ConfigurationServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationService _configurationService;
  private readonly IEventSerializer _eventSerializer;

  public ConfigurationServiceTests() : base()
  {
    _cacheService = ServiceProvider.GetRequiredService<ICacheService>();
    _configurationService = ServiceProvider.GetRequiredService<IConfigurationService>();
    _eventSerializer = ServiceProvider.GetRequiredService<IEventSerializer>();

    InitializeConfiguration = false;
  }

  [Fact(DisplayName = "InitializeAsync: it should initialize the configuration.")]
  public async Task InitializeAsync_it_should_initialize_the_configuration()
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
      },
      Session = new SessionPayload
      {
        CustomAttributes = new CustomAttribute[]
        {
          new("IpAddress", Faker.Internet.IpAddress().ToString()),
          new("UserAgent", Faker.Internet.UserAgent())
        }
      }
    };

    InitializeConfigurationResult result = await _configurationService.InitializeAsync(payload);
    Session session = result.Session;
    User user = session.User;

    Actor actor = new()
    {
      Id = user.Id,
      Type = ActorType.User,
      DisplayName = user.FullName ?? user.UniqueName,
      EmailAddress = user.Email?.Address,
      PictureUrl = user.Picture
    };
    ActorId actorId = new(user.Id);

    Assert.NotEqual(default, session.Id);
    Assert.Equal(actor, session.CreatedBy);
    AssertIsNear(session.CreatedOn);
    Assert.Equal(actor, session.UpdatedBy);
    AssertIsNear(session.UpdatedOn);
    Assert.True(session.Version >= 1);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.Session.CustomAttributes, session.CustomAttributes);

    Assert.NotEqual(default, user.Id);
    Assert.Equal(actor, user.CreatedBy);
    AssertIsNear(user.CreatedOn);
    Assert.Equal(actor, user.UpdatedBy);
    AssertIsNear(user.UpdatedOn);
    Assert.True(user.Version >= 1);

    Assert.Equal(payload.User.UniqueName, user.UniqueName);
    Assert.Equal(payload.User.EmailAddress, user.Email?.Address);
    Assert.Equal(payload.User.FirstName, user.FirstName);
    Assert.Equal(payload.User.LastName, user.LastName);
    Assert.Equal(payload.Locale, user.Locale?.Code);

    await AssertUserPasswordAsync(user.Id);

    ConfigurationAggregate? configuration = _cacheService.Configuration;
    Assert.NotNull(configuration);

    Assert.Equal(ConfigurationAggregate.UniqueId, configuration.Id);
    Assert.Equal(actorId, configuration.CreatedBy);
    AssertIsNear(configuration.CreatedOn);
    Assert.Equal(actorId, configuration.UpdatedBy);
    AssertIsNear(configuration.UpdatedOn);
    Assert.Equal(1, configuration.Version);

    Assert.Equal(payload.Locale, configuration.DefaultLocale.Code);
    Assert.Equal(32, configuration.Secret.Length);
    Assert.Equal(new ReadOnlyUniqueNameSettings(), configuration.UniqueNameSettings);
    Assert.Equal(new ReadOnlyPasswordSettings(), configuration.PasswordSettings);
    Assert.Equal(new ReadOnlyLoggingSettings(), configuration.LoggingSettings);
  }

  [Fact(DisplayName = "InitializeAsync: it should throw ConfigurationAlreadyInitializedException when the configuration is already initialized.")]
  public async Task InitializeAsync_it_should_throw_ConfigurationAlreadyInitializedException_when_the_configuration_is_already_initialized()
  {
    await InitializeConfigurationAsync();

    InitializeConfigurationPayload payload = new();

    await Assert.ThrowsAsync<ConfigurationAlreadyInitializedException>(async () => await _configurationService.InitializeAsync(payload));
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the configuration has not been initialized.")]
  public async Task ReadAsync_it_should_return_null_when_the_configuration_has_not_been_initialized()
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

  [Fact(DisplayName = "ReplaceAsync: it should replace the configuration.")]
  public async Task ReplaceAsync_it_should_replace_the_configuration()
  {
    await InitializeConfigurationAsync();

    Assert.NotNull(Configuration);
    string oldSecret = Configuration.Secret;

    ReplaceConfigurationPayload payload = new()
    {
      DefaultLocale = "fr-CA",
      Secret = string.Empty,
      UniqueNameSettings = new UniqueNameSettings
      {
        AllowedCharacters = Configuration.UniqueNameSettings.AllowedCharacters
      },
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 7,
        RequiredUniqueChars = 4,
        RequireNonAlphanumeric = false,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = true,
        Strategy = "PBKDF2"
      },
      LoggingSettings = new LoggingSettings
      {
        Extent = Configuration.LoggingSettings.Extent,
        OnlyErrors = Configuration.LoggingSettings.OnlyErrors
      }
    };

    Configuration configuration = await _configurationService.ReplaceAsync(payload, Configuration.Version);

    Assert.Equal(Actor, configuration.UpdatedBy);
    AssertIsNear(configuration.UpdatedOn);
    Assert.True(configuration.Version > 1);

    Assert.Equal(payload.DefaultLocale, configuration.DefaultLocale.Code);
    Assert.NotEqual(oldSecret, configuration.Secret);
    Assert.Equal(payload.PasswordSettings, configuration.PasswordSettings);

    EventEntity? @event = await EventContext.Events.AsNoTracking()
      .Where(e => e.AggregateType == typeof(ConfigurationAggregate).GetName()
        && e.AggregateId == ConfigurationAggregate.UniqueId.Value
        && e.EventType == typeof(ConfigurationUpdatedEvent).GetName()
      )
      .OrderBy(e => e.OccurredOn)
      .LastOrDefaultAsync();
    Assert.NotNull(@event);
    ConfigurationUpdatedEvent updated = (ConfigurationUpdatedEvent)_eventSerializer.Deserialize(@event);
    Assert.Null(updated.UniqueNameSettings);
    Assert.Null(updated.LoggingSettings);
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw InvalidOperationException when the configuration has not been initialized.")]
  public async Task ReplaceAsync_it_should_throw_InvalidOperationException_when_the_configuration_has_not_been_initialized()
  {
    ReplaceConfigurationPayload payload = new();
    await Assert.ThrowsAsync<InvalidOperationException>(async () => await _configurationService.ReplaceAsync(payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should throw InvalidOperationException when the configuration has not been initialized.")]
  public async Task UpdateAsync_it_should_throw_InvalidOperationException_when_the_configuration_has_not_been_initialized()
  {
    UpdateConfigurationPayload payload = new();
    await Assert.ThrowsAsync<InvalidOperationException>(async () => await _configurationService.UpdateAsync(payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should update the configuration.")]
  public async Task UpdateAsync_it_should_update_the_configuration()
  {
    await InitializeConfigurationAsync();

    Assert.NotNull(Configuration);
    string oldSecret = Configuration.Secret;

    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    ReadOnlyLoggingSettings loggingSettings = new();

    UpdateConfigurationPayload payload = new()
    {
      DefaultLocale = "fr-CA",
      Secret = string.Empty,
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = 7,
        RequiredUniqueChars = 4,
        RequireNonAlphanumeric = false,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = true,
        Strategy = "PBKDF2"
      }
    };

    Configuration configuration = await _configurationService.UpdateAsync(payload);

    Assert.Equal(Actor, configuration.UpdatedBy);
    AssertIsNear(configuration.UpdatedOn);
    Assert.True(configuration.Version > 1);

    Assert.Equal(payload.DefaultLocale, configuration.DefaultLocale.Code);
    Assert.NotEqual(oldSecret, configuration.Secret);
    Assert.Equal(uniqueNameSettings.AllowedCharacters, configuration.UniqueNameSettings.AllowedCharacters);
    Assert.Equal(payload.PasswordSettings, configuration.PasswordSettings);
    Assert.Equal(loggingSettings.Extent, configuration.LoggingSettings.Extent);
    Assert.Equal(loggingSettings.OnlyErrors, configuration.LoggingSettings.OnlyErrors);
  }
}
