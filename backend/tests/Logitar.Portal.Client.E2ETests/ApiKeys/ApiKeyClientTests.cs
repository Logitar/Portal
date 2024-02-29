using Logitar.Net.Http;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.ApiKeys;

internal class ApiKeyClientTests : IClientTests
{
  private readonly IApiKeyClient _client;

  public ApiKeyClientTests(IApiKeyClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      DateTime expiresOn = DateTime.Now.AddYears(1);
      CreateApiKeyPayload create = new("Test API Key")
      {
        ExpiresOn = expiresOn
      };
      ApiKey apiKey = await _client.CreateAsync(create, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.DeleteAsync));
      apiKey = await _client.DeleteAsync(apiKey.Id, context.Request)
        ?? throw new InvalidOperationException("The API key should not be null.");
      apiKey = await _client.CreateAsync(create, context.Request);
      if (apiKey.XApiKey == null)
      {
        throw new InvalidOperationException("The X-API-Key should not be null.");
      }
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.AuthenticateAsync));
      AuthenticateApiKeyPayload authenticate = new(apiKey.XApiKey);
      apiKey = await _client.AuthenticateAsync(authenticate, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      ApiKey? notFound = await _client.ReadAsync(Guid.NewGuid(), context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The API key should not be found.");
      }
      apiKey = await _client.ReadAsync(apiKey.Id, context.Request)
        ?? throw new InvalidOperationException("The API key should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SearchAsync));
      SearchApiKeysPayload search = new()
      {
        HasAuthenticated = true,
        Status = new ApiKeyStatus(isExpired: false, DateTime.Now)
      };
      search.Search.Terms.Add(new SearchTerm("test%"));
      SearchResults<ApiKey> results = await _client.SearchAsync(search, context.Request);
      apiKey = results.Items.Single();
      context.Succeed();

      long version = apiKey.Version;

      context.SetName(_client.GetType(), nameof(_client.UpdateAsync));
      UpdateApiKeyPayload update = new()
      {
        ExpiresOn = DateTime.Now.AddMonths(6)
      };
      if (context.Role == null)
      {
        throw new InvalidOperationException("The role should not be null in the context.");
      }
      update.Roles.Add(new RoleModification(context.Role.UniqueName));
      apiKey = await _client.UpdateAsync(apiKey.Id, update, context.Request)
        ?? throw new InvalidOperationException("The API key should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReplaceAsync));
      ReplaceApiKeyPayload replace = new(apiKey.DisplayName)
      {
        Description = apiKey.Description,
        ExpiresOn = expiresOn,
        CustomAttributes = apiKey.CustomAttributes,
        Roles = apiKey.Roles.Select(role => role.UniqueName).ToList()
      };
      apiKey = await _client.ReplaceAsync(apiKey.Id, replace, version, context.Request)
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
