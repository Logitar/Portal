using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

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
    const int millisecondsDelay = 500;

    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out _);
    ApiKey notInRealm = new(new DisplayName("Portal API Key"), secret);
    ApiKey notInSearch = new(new DisplayName("Default"), secret, id: ApiKeyId.NewId(TenantId));
    ApiKey notInIds = new(new DisplayName("Other API Key"), secret, id: ApiKeyId.NewId(TenantId));
    ApiKey expired = new(new DisplayName("Expired API Key"), secret, id: ApiKeyId.NewId(TenantId))
    {
      ExpiresOn = DateTime.Now.AddMilliseconds(millisecondsDelay)
    };
    expired.Update();
    ApiKey apiKey1 = new(new DisplayName("First API Key"), secret, id: ApiKeyId.NewId(TenantId))
    {
      ExpiresOn = DateTime.Now.AddDays(90)
    };
    apiKey1.Update();
    ApiKey apiKey2 = new(new DisplayName("Second API Key"), secret, id: ApiKeyId.NewId(TenantId))
    {
      ExpiresOn = DateTime.Now.AddYears(1)
    };
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
    IEnumerable<Guid> apiKeyIds = (await _apiKeyRepository.LoadAsync()).Select(apiKey => apiKey.EntityId.ToGuid());
    payload.Ids.AddRange(apiKeyIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.EntityId.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("%key"));
    payload.Sort.Add(new ApiKeySortOption(ApiKeySort.ExpiresOn, isDescending: false));
    SearchApiKeysQuery query = new(payload);
    SearchResults<ApiKeyModel> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    ApiKeyModel apiKey = Assert.Single(results.Items);
    Assert.Equal(apiKey2.EntityId.ToGuid(), apiKey.Id);
  }
}
