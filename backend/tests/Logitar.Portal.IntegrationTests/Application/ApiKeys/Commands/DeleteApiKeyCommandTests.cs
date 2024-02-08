using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteApiKeyCommandTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;

  public DeleteApiKeyCommandTests()
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
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should delete an existing API key.")]
  public async Task It_should_delete_an_existing_Api_key()
  {
    ApiKeyAggregate apiKey = await CreateApiKeyAsync();

    DeleteApiKeyCommand command = new(apiKey.Id.AggregateId.ToGuid());
    ApiKey? deleted = await Mediator.Send(command);
    Assert.NotNull(deleted);
    Assert.Equal(command.Id, deleted.Id);
  }

  [Fact(DisplayName = "It should return null when the API key cannot be found.")]
  public async Task It_should_return_null_when_the_Api_key_cannot_be_found()
  {
    DeleteApiKeyCommand command = new(Guid.NewGuid());
    ApiKey? apiKey = await Mediator.Send(command);
    Assert.Null(apiKey);
  }

  [Fact(DisplayName = "It should return null when the API key is in another tenant.")]
  public async Task It_should_return_null_when_the_Api_key_is_in_another_tenant()
  {
    ApiKeyAggregate apiKey = await CreateApiKeyAsync();

    SetRealm();

    DeleteApiKeyCommand command = new(apiKey.Id.AggregateId.ToGuid());
    ApiKey? result = await Mediator.Send(command);
    Assert.Null(result);
  }

  private async Task<ApiKeyAggregate> CreateApiKeyAsync()
  {
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out _);
    ApiKeyAggregate apiKey = new(new DisplayNameUnit("Default"), secret);

    await _apiKeyRepository.SaveAsync(apiKey);

    return apiKey;
  }
}
