using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Contracts.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.ApiKeys.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadApiKeyQueryTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;

  public ReadApiKeyQueryTests() : base()
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

  [Fact(DisplayName = "It should return null when the API key cannot be found.")]
  public async Task It_should_return_null_when_the_Api_key_cannot_be_found()
  {
    ReadApiKeyQuery query = new(Guid.NewGuid());
    ApiKeyModel? apiKey = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(apiKey);
  }

  [Fact(DisplayName = "It should return the API key when it is found.")]
  public async Task It_should_return_the_Api_key_when_it_is_found()
  {
    ApiKey apiKey = await CreateApiKeyAsync();

    ReadApiKeyQuery query = new(apiKey.EntityId.ToGuid());
    ApiKeyModel? result = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(result);
    Assert.Equal(query.Id, result.Id);
  }

  private async Task<ApiKey> CreateApiKeyAsync()
  {
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out _);
    ApiKey apiKey = new(new DisplayName("Default"), secret);

    await _apiKeyRepository.SaveAsync(apiKey);

    return apiKey;
  }
}
