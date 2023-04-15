using Bogus;
using Logitar.Portal.Core.Configurations;

namespace Logitar.Portal.Client;

internal class ConfigurationServiceTests
{
  private readonly Faker _faker = new();

  private readonly IConfigurationService _configurationService;

  public ConfigurationServiceTests(IConfigurationService configurationService)
  {
    _configurationService = configurationService;
  }

  public async Task ExecuteAsync(TestContext context, Credentials credentials, CancellationToken cancellationToken = default)
  {
    string name = string.Empty;
    try
    {
      name = string.Join('.', nameof(ConfigurationService), nameof(ConfigurationService.InitializeAsync));
      _ = await _configurationService.InitializeAsync(new InitializeConfigurationInput
      {
        DefaultLocale = "en",
        LoggingSettings = new LoggingSettings
        {
          Extent = LoggingExtent.ActivityOnly
        },
        User = new InitialUserInput
        {
          Username = credentials.Username,
          Password = credentials.Password,
          EmailAddress = _faker.Person.Email,
          FirstName = _faker.Person.FirstName,
          LastName = _faker.Person.LastName
        }
      }, cancellationToken);
      context.Succeed(name);
    }
    catch (Exception exception)
    {
      context.Fail(name, exception);
    }
  }
}
