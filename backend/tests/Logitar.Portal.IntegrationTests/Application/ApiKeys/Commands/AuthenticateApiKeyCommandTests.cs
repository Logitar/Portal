using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class AuthenticateApiKeyCommandTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;

  private string? _secret = null;

  public AuthenticateApiKeyCommandTests() : base()
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

  [Fact(DisplayName = "It should authenticate the API key.")]
  public async Task It_should_authenticate_the_Api_key()
  {
    SetUser(user: null);

    ApiKey apiKey = await CreateApiKeyAsync();
    Assert.NotNull(_secret);

    apiKey.ExpiresOn = DateTime.Now.AddDays(1);
    apiKey.Update();
    await _apiKeyRepository.SaveAsync(apiKey);

    AuthenticateApiKeyPayload payload = new(XApiKey.Encode(apiKey.Id, _secret));
    AuthenticateApiKeyCommand command = new(payload);
    ApiKeyModel result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Equal(apiKey.EntityId.ToGuid(), result.Id);

    apiKey = Assert.Single(await _apiKeyRepository.LoadAsync());
    Assert.Equal(apiKey.Id.Value, apiKey.UpdatedBy?.Value);
    Assert.NotNull(apiKey.AuthenticatedOn);
    Assert.Equal(DateTime.Now, apiKey.AuthenticatedOn.Value, TimeSpan.FromSeconds(15));
  }

  [Fact(DisplayName = "It should throw ApiKeyIsExpiredException when the Api key is expired.")]
  public async Task It_should_throw_ApiKeyIsExpiredException_when_the_Api_key_is_expired()
  {
    const int millisecondsDelay = 50;

    ApiKey apiKey = await CreateApiKeyAsync();
    Assert.NotNull(_secret);

    apiKey.ExpiresOn = DateTime.UtcNow.AddMilliseconds(millisecondsDelay);
    apiKey.Update();
    await _apiKeyRepository.SaveAsync(apiKey);

    await Task.Delay(millisecondsDelay);

    AuthenticateApiKeyPayload payload = new(XApiKey.Encode(apiKey.Id, _secret));
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ApiKeyIsExpiredException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(apiKey.Id.Value, exception.ApiKeyId);
  }

  [Fact(DisplayName = "It should throw ApiKeyNotFoundException when the API key could not be found.")]
  public async Task It_should_throw_ApiKeyNotFoundException_when_the_Api_key_could_not_be_found()
  {
    XApiKey xApiKey = new(ApiKeyId.NewId(), RandomStringGenerator.GetBase64String(XApiKey.SecretLength, out _));
    AuthenticateApiKeyPayload payload = new(xApiKey.Encode());
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ApiKeyNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(xApiKey.Id.Value, exception.Id);
    Assert.Equal("XApiKey", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ApiKeyNotFoundException when the API key is in another realm.")]
  public async Task It_should_throw_ApiKeyNotFoundException_when_the_Api_key_is_in_another_realm()
  {
    SetRealm();

    ApiKey apiKey = await CreateApiKeyAsync();
    Assert.NotNull(_secret);

    string xApiKey = XApiKey.Encode(apiKey.Id, _secret);
    AuthenticateApiKeyPayload payload = new(xApiKey);
    AuthenticateApiKeyCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<ApiKeyNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(apiKey.Id.Value, exception.Id);
    Assert.Equal("XApiKey", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw IncorrectApiKeySecretException when the secret is incorrect.")]
  public async Task It_should_throw_IncorrectApiKeySecretException_when_the_secret_is_incorrect()
  {
    ApiKey apiKey = await CreateApiKeyAsync();
    _ = _passwordManager.GenerateBase64(XApiKey.SecretLength, out string secret);
    Assert.NotEqual(_secret, secret);

    AuthenticateApiKeyPayload payload = new(XApiKey.Encode(apiKey.Id, secret));
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectApiKeySecretException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(apiKey.Id.Value, exception.ApiKeyId);
    Assert.Equal(secret, exception.AttemptedSecret);
  }

  [Fact(DisplayName = "It should throw InvalidApiKeyException when the API key is not valid.")]
  public async Task It_should_throw_InvalidApiKeyException_when_the_Api_key_is_not_valid()
  {
    AuthenticateApiKeyPayload payload = new("test");
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidApiKeyException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.XApiKey, exception.ApiKey);
    Assert.Equal("XApiKey", exception.PropertyName);
    Assert.NotNull(exception.InnerException);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    AuthenticateApiKeyPayload payload = new(string.Empty);
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("XApiKey", exception.Errors.Single().PropertyName);
  }

  private async Task<ApiKey> CreateApiKeyAsync()
  {
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out _secret);
    ApiKey apiKey = new(new DisplayName("Default"), secret, id: ApiKeyId.NewId(TenantId));

    await _apiKeyRepository.SaveAsync(apiKey);

    return apiKey;
  }
}
