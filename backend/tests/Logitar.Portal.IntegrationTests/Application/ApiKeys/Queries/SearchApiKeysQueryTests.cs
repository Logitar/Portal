using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.ApiKeys.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchApiKeysQueryTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;

  public SearchApiKeysQueryTests() : base()
  {
    _apiKeyRepository = ServiceProvider.GetRequiredService<IApiKeyRepository>();
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.ApiKeys.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should return empty results when no API key did match.")]
  public async Task It_should_return_empty_results_when_no_Api_key_did_match()
  {
    SearchApiKeysPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("%test%"));
    SearchApiKeysQuery query = new(payload);
    SearchResults<ApiKeyModel> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    const int millisecondsDelay = 50;

    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out _);
    ApiKey notInRealm = new(new DisplayName("Portal API Key"), secret, tenantId: null);
    ApiKey notInSearch = new(new DisplayName("Default"), secret, TenantId);
    ApiKey notInIds = new(new DisplayName("Other API Key"), secret, TenantId);
    ApiKey expired = new(new DisplayName("Expired API Key"), secret, TenantId);
    expired.SetExpiration(DateTime.Now.AddMilliseconds(millisecondsDelay));
    expired.Update();
    ApiKey apiKey1 = new(new DisplayName("First API Key"), secret, TenantId);
    apiKey1.SetExpiration(DateTime.Now.AddDays(90));
    apiKey1.Update();
    ApiKey apiKey2 = new(new DisplayName("Second API Key"), secret, TenantId);
    apiKey2.SetExpiration(DateTime.Now.AddYears(1));
    apiKey2.Update();
    await _apiKeyRepository.SaveAsync([notInRealm, notInSearch, notInIds, expired, apiKey1, apiKey2]);

    await Task.Delay(millisecondsDelay);

    SetRealm();

    SearchApiKeysPayload payload = new()
    {
      Status = new ApiKeyStatus(isExpired: false),
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> apiKeyIds = (await _apiKeyRepository.LoadAsync()).Select(apiKey => apiKey.Id.ToGuid());
    payload.Ids.AddRange(apiKeyIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.Id.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("%key"));
    payload.Sort.Add(new ApiKeySortOption(ApiKeySort.ExpiresOn, isDescending: false));
    SearchApiKeysQuery query = new(payload);
    SearchResults<ApiKeyModel> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    ApiKeyModel apiKey = Assert.Single(results.Items);
    Assert.Equal(apiKey2.Id.ToGuid(), apiKey.Id);
  }
}
