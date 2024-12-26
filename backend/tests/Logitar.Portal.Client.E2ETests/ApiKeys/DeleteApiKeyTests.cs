using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.ApiKeys;

internal class DeleteApiKeyTests : IClientTests
{
  private readonly IApiKeyClient _client;

  public DeleteApiKeyTests(IApiKeyClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(GetType(), nameof(ExecuteAsync));
      if (StaticPortalSettings.Instance.ApiKey == null)
      {
        throw new InvalidOperationException("The X-API-Key should not be null.");
      }
      string[] parts = StaticPortalSettings.Instance.ApiKey.Split('.');
      Guid id = new(Convert.FromBase64String(parts[1].FromUriSafeBase64()));
      ApiKeyModel apiKey = await _client.DeleteAsync(id, context.Request)
        ?? throw new InvalidOperationException("The API key should not be null.");
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
