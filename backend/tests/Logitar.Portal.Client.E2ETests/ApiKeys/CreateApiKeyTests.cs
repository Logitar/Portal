using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.ApiKeys;

internal class CreateApiKeyTests : IClientTests
{
  private readonly IApiKeyClient _client;

  public CreateApiKeyTests(IApiKeyClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(GetType(), nameof(ExecuteAsync));
      CreateApiKeyPayload payload = new("Test API Key");
      ApiKeyModel apiKey = await _client.CreateAsync(payload, context.Request);
      StaticPortalSettings.Instance.ApiKey = apiKey.XApiKey ?? throw new InvalidOperationException("The X-API-Key should not be null.");
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
