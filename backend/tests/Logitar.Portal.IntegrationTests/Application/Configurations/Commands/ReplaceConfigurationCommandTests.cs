using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Configurations.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceConfigurationCommandTests : IntegrationTests
{
  private readonly IConfigurationRepository _configurationRepository;

  public ReplaceConfigurationCommandTests()
  {
    _configurationRepository = ServiceProvider.GetRequiredService<IConfigurationRepository>();
  }

  [Fact(DisplayName = "It should replace the configuration.")]
  public async Task It_should_replace_the_configuration()
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync();
    Assert.NotNull(configuration);
    string oldSecret = configuration.Secret.Value;
    long version = configuration.Version;

    configuration.Secret = JwtSecretUnit.Generate();
    string newSecret = configuration.Secret.Value;
    configuration.Update(default);
    await _configurationRepository.SaveAsync(configuration);

    ReplaceConfigurationPayload payload = new(oldSecret)
    {
      DefaultLocale = "fr-CA",
      UniqueNameSettings = new UniqueNameSettings("abcdeéfghijklmnopqrstuvwxyzABCDEÉFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"),
      PasswordSettings = new PasswordSettings(requiredLength: 7, requiredUniqueChars: 3, requireNonAlphanumeric: false, requireLowercase: true, requireUppercase: true, requireDigit: true, hashingStrategy: "PBKDF2"),
      RequireUniqueEmail = false,
      LoggingSettings = new LoggingSettings(LoggingExtent.Full, onlyErrors: true)
    };
    ReplaceConfigurationCommand command = new(payload, version);
    Configuration result = await Mediator.Send(command);

    Assert.Equal(payload.DefaultLocale, result.DefaultLocale);
    Assert.Equal(newSecret, result.Secret);
    Assert.Equal(payload.UniqueNameSettings, result.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, result.PasswordSettings);
    Assert.Equal(payload.RequireUniqueEmail, result.RequireUniqueEmail);
    Assert.Equal(payload.LoggingSettings, result.LoggingSettings);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceConfigurationPayload payload = new()
    {
      DefaultLocale = "test"
    };
    ReplaceConfigurationCommand command = new(payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("DefaultLocale", exception.Errors.Single().PropertyName);
  }
}
