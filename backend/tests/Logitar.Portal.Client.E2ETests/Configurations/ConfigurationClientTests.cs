using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Configurations;

internal class ConfigurationClientTests : IClientTests
{
  private readonly IConfigurationClient _client;

  public ConfigurationClientTests(IConfigurationClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      ConfigurationModel configuration = await _client.ReadAsync(context.Request);
      context.Succeed();

      long version = configuration.Version;

      context.SetName(_client.GetType(), nameof(_client.UpdateAsync));
      UpdateConfigurationPayload update = new()
      {
        DefaultLocale = new Modification<string>("fr-CA")
      };
      configuration = await _client.UpdateAsync(update, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReplaceAsync));
      ReplaceConfigurationPayload replace = new(configuration.Secret)
      {
        DefaultLocale = "fr",
        UniqueNameSettings = configuration.UniqueNameSettings,
        PasswordSettings = configuration.PasswordSettings,
        RequireUniqueEmail = configuration.RequireUniqueEmail,
        LoggingSettings = new LoggingSettings(LoggingExtent.Full, onlyErrors: true)
      };
      configuration = await _client.ReplaceAsync(replace, version, context.Request);
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
