using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Configurations.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateConfigurationCommandTests : IntegrationTests
{
  private readonly IConfigurationRepository _configurationRepository;

  public UpdateConfigurationCommandTests() : base()
  {
    _configurationRepository = ServiceProvider.GetRequiredService<IConfigurationRepository>();
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateConfigurationPayload payload = new()
    {
      DefaultLocale = new ChangeModel<string>("test")
    };
    UpdateConfigurationCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("DefaultLocale.Value", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should update the configuration.")]
  public async Task It_should_update_the_configuration()
  {
    Configuration? configuration = await _configurationRepository.LoadAsync();
    Assert.NotNull(configuration);
    string secret = configuration.Secret.Value;

    UpdateConfigurationPayload payload = new()
    {
      DefaultLocale = new ChangeModel<string>("fr-CA"),
      Secret = "    ",
      UniqueNameSettings = new UniqueNameSettings("abcdeéfghijklmnopqrstuvwxyzABCDEÉFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"),
      PasswordSettings = new PasswordSettings(requiredLength: 7, requiredUniqueChars: 3, requireNonAlphanumeric: false, requireLowercase: true, requireUppercase: true, requireDigit: true, hashingStrategy: "PBKDF2"),
      RequireUniqueEmail = false,
      LoggingSettings = new LoggingSettings(LoggingExtent.Full, onlyErrors: true)
    };
    UpdateConfigurationCommand command = new(payload);
    ConfigurationModel result = await ActivityPipeline.ExecuteAsync(command);

    Assert.Equal(payload.DefaultLocale.Value, result.DefaultLocale?.Code);
    Assert.NotEqual(secret, result.Secret);
    Assert.Equal(payload.UniqueNameSettings, result.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, result.PasswordSettings);
    Assert.Equal(payload.RequireUniqueEmail, result.RequireUniqueEmail);
    Assert.Equal(payload.LoggingSettings, result.LoggingSettings);
  }
}
