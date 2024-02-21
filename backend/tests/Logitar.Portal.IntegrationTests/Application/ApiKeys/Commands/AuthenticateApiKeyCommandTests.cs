using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should authenticate the API key.")]
  public async Task It_should_authenticate_the_Api_key()
  {
    ApiKeyAggregate apiKey = await CreateApiKeyAsync();
    Assert.NotNull(_secret);

    apiKey.SetExpiration(DateTime.Now.AddDays(1));
    apiKey.Update();
    await _apiKeyRepository.SaveAsync(apiKey);

    AuthenticateApiKeyPayload payload = new(XApiKey.Encode(apiKey.Id, _secret));
    AuthenticateApiKeyCommand command = new(payload);
    ApiKey result = await Mediator.Send(command);
    Assert.Equal(apiKey.Id.ToGuid(), result.Id);

    apiKey = Assert.Single(await _apiKeyRepository.LoadAsync());
    Assert.Equal(apiKey.Id.Value, apiKey.UpdatedBy.Value);
    Assert.NotNull(apiKey.AuthenticatedOn);
    Assert.Equal(DateTime.Now, apiKey.AuthenticatedOn.Value, TimeSpan.FromSeconds(15));
  }

  [Fact(DisplayName = "It should throw ApiKeyIsExpiredException when the Api key is expired.")]
  public async Task It_should_throw_ApiKeyIsExpiredException_when_the_Api_key_is_expired()
  {
    const int millisecondsDelay = 50;

    ApiKeyAggregate apiKey = await CreateApiKeyAsync();
    Assert.NotNull(_secret);

    apiKey.SetExpiration(DateTime.Now.AddMilliseconds(millisecondsDelay));
    apiKey.Update();
    await _apiKeyRepository.SaveAsync(apiKey);

    await Task.Delay(millisecondsDelay);

    AuthenticateApiKeyPayload payload = new(XApiKey.Encode(apiKey.Id, _secret));
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ApiKeyIsExpiredException>(async () => await Mediator.Send(command));
    Assert.Equal(apiKey.Id, exception.ApiKeyId);
  }

  [Fact(DisplayName = "It should throw ApiKeyNotFoundException when the API key could not be found.")]
  public async Task It_should_throw_ApiKeyNotFoundException_when_the_Api_key_could_not_be_found()
  {
    XApiKey xApiKey = new(ApiKeyId.NewId(), RandomStringGenerator.GetBase64String(XApiKey.SecretLength, out _));
    AuthenticateApiKeyPayload payload = new(xApiKey.Encode());
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ApiKeyNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(xApiKey.Id, exception.Id);
    Assert.Equal("XApiKey", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ApiKeyNotFoundException when the API key is in another realm.")]
  public async Task It_should_throw_ApiKeyNotFoundException_when_the_Api_key_is_in_another_realm()
  {
    SetRealm();

    ApiKeyAggregate apiKey = await CreateApiKeyAsync();
    Assert.NotNull(_secret);

    string xApiKey = XApiKey.Encode(apiKey.Id, _secret);
    AuthenticateApiKeyPayload payload = new(xApiKey);
    AuthenticateApiKeyCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<ApiKeyNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(apiKey.Id, exception.Id);
    Assert.Equal("XApiKey", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw IncorrectApiKeySecretException when the secret is incorrect.")]
  public async Task It_should_throw_IncorrectApiKeySecretException_when_the_secret_is_incorrect()
  {
    ApiKeyAggregate apiKey = await CreateApiKeyAsync();
    _ = _passwordManager.GenerateBase64(XApiKey.SecretLength, out string secret);
    Assert.NotEqual(_secret, secret);

    AuthenticateApiKeyPayload payload = new(XApiKey.Encode(apiKey.Id, secret));
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectApiKeySecretException>(async () => await Mediator.Send(command));
    Assert.Equal(apiKey.Id, exception.ApiKeyId);
    Assert.Equal(secret, exception.AttemptedSecret);
  }

  [Fact(DisplayName = "It should throw InvalidApiKeyException when the API key is not valid.")]
  public async Task It_should_throw_InvalidApiKeyException_when_the_Api_key_is_not_valid()
  {
    AuthenticateApiKeyPayload payload = new("test");
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidApiKeyException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.XApiKey, exception.ApiKey);
    Assert.Equal("XApiKey", exception.PropertyName);
    Assert.NotNull(exception.InnerException);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    AuthenticateApiKeyPayload payload = new(string.Empty);
    AuthenticateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("XApiKey", exception.Errors.Single().PropertyName);
  }

  private async Task<ApiKeyAggregate> CreateApiKeyAsync()
  {
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out _secret);
    ApiKeyAggregate apiKey = new(new DisplayNameUnit("Default"), secret);

    await _apiKeyRepository.SaveAsync(apiKey);

    return apiKey;
  }
}
