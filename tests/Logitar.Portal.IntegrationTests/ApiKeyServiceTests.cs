using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class ApiKeyServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IApiKeyService _apiKeyService;

  private readonly RealmAggregate _realm;
  private readonly RoleAggregate _readUsers;
  private readonly RoleAggregate _writeUsers;
  private readonly byte[] _secret;
  private readonly ApiKeyAggregate _apiKey;

  public ApiKeyServiceTests() : base()
  {
    _apiKeyService = ServiceProvider.GetRequiredService<IApiKeyService>();

    _realm = new("desjardins")
    {
      DisplayName = "Desjardins",
      DefaultLocale = new Locale(Faker.Locale),
      Url = new Uri("https://www.desjardins.com/")
    };

    _readUsers = new(_realm.UniqueNameSettings, "read_users", _realm.Id.Value);
    _writeUsers = new(_realm.UniqueNameSettings, "write_users", _realm.Id.Value);

    Password secret = PasswordService.Generate(_realm.PasswordSettings, ApiKeyAggregate.SecretLength, out _secret);
    _apiKey = new("Default", secret, _realm.Id.Value)
    {
      Description = "This is the default API key.",
      ExpiresOn = DateTime.Now.AddYears(1)
    };
    _apiKey.AddRole(_readUsers);
    _apiKey.AddRole(_writeUsers);
    _apiKey.SetCustomAttribute("Key1", "Value1");
    _apiKey.SetCustomAttribute("Key2", "Value2");
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _readUsers, _writeUsers, _apiKey });
  }

  [Fact(DisplayName = "AuthenticateAsync: it should authenticate the Portal API key.")]
  public async Task AuthenticateAsync_it_should_authenticate_the_Portal_Api_key()
  {
    Password secret = PasswordService.Generate(new ReadOnlyPasswordSettings(), ApiKeyAggregate.SecretLength, out byte[] secretBytes);
    ApiKeyAggregate aggregate = new("Default", secret);
    await AggregateRepository.SaveAsync(aggregate);

    XApiKey xApiKey = new(aggregate, secretBytes);

    ApiKey apiKey = await _apiKeyService.AuthenticateAsync(xApiKey.Encode());

    Assert.Equal(aggregate.Id.ToGuid(), apiKey.Id);

    Assert.Equal(Actor, apiKey.UpdatedBy);
    AssertIsNear(apiKey.UpdatedOn);
    Assert.True(apiKey.Version > 1);

    Assert.Equal(apiKey.UpdatedOn, apiKey.AuthenticatedOn);

    Assert.Null(apiKey.Realm);
  }

  [Fact(DisplayName = "AuthenticateAsync: it should authenticate the realm API key.")]
  public async Task AuthenticateAsync_it_should_authenticate_the_realm_Api_key()
  {
    XApiKey xApiKey = new(_apiKey, _secret);

    ApiKey apiKey = await _apiKeyService.AuthenticateAsync(xApiKey.Encode());

    Assert.Equal(_apiKey.Id.ToGuid(), apiKey.Id);

    Assert.Equal(Actor, apiKey.UpdatedBy);
    AssertIsNear(apiKey.UpdatedOn);
    Assert.True(apiKey.Version > 1);

    Assert.Equal(apiKey.UpdatedOn, apiKey.AuthenticatedOn);

    Assert.NotNull(apiKey.Realm);
    Assert.Equal(_realm.Id.ToGuid(), apiKey.Realm.Id);
  }

  [Fact(DisplayName = "AuthenticateAsync: it should throw ApiKeyNotFoundException when the API key could not be found.")]
  public async Task AuthenticateAsync_it_should_throw_ApiKeyNotFoundException_when_the_Api_key_could_not_be_found()
  {
    Password secret = PasswordService.Generate(new ReadOnlyPasswordSettings(), ApiKeyAggregate.SecretLength, out byte[] secretBytes);
    ApiKeyAggregate apiKey = new("Default", secret);
    XApiKey xApiKey = new(apiKey, secretBytes);

    var exception = await Assert.ThrowsAsync<ApiKeyNotFoundException>(async () => await _apiKeyService.AuthenticateAsync(xApiKey.Encode()));
    Assert.Equal(xApiKey.Id.Value, exception.Id);
  }

  [Fact(DisplayName = "AuthenticateAsync: it should throw InvalidXApiKeyException when the X-API-Key is not valid.")]
  public async Task AuthenticateAsync_it_should_throw_InvalidXApiKeyException_when_the_X_Api_Key_is_not_valid()
  {
    string xApiKey = "PT:abc:123";
    var exception = await Assert.ThrowsAsync<InvalidXApiKeyException>(async () => await _apiKeyService.AuthenticateAsync(xApiKey));
    Assert.Equal(xApiKey, exception.XApiKey);
  }

  [Fact(DisplayName = "CreateAsync: it should create a new Portal API key.")]
  public async Task CreateAsync_it_should_create_a_new_Portal_Api_key()
  {
    CreateApiKeyPayload payload = new()
    {
      Title = "  Default  ",
      Description = "    ",
      ExpiresOn = DateTime.Now.AddYears(1),
      CustomAttributes = new CustomAttribute[]
      {
        new("  Key1  ", "  Value1  "),
        new("Key2", "Value2")
      }
    };

    ApiKey? apiKey = await _apiKeyService.CreateAsync(payload);
    Assert.NotNull(apiKey);

    Assert.NotEqual(Guid.Empty, apiKey.Id);
    Assert.Equal(Actor, apiKey.CreatedBy);
    AssertIsNear(apiKey.CreatedOn);
    Assert.Equal(apiKey.CreatedBy, apiKey.UpdatedBy);
    Assert.Equal(apiKey.CreatedBy, apiKey.UpdatedBy);
    Assert.True(apiKey.Version >= 1);

    Assert.Equal(payload.Title.Trim(), apiKey.Title);
    Assert.Equal(payload.Description?.CleanTrim(), apiKey.Description);
    AssertEqual(payload.ExpiresOn, apiKey.ExpiresOn);

    Assert.Equal(2, apiKey.CustomAttributes.Count());
    Assert.Contains(apiKey.CustomAttributes, customAttribute => customAttribute.Key == "Key1" && customAttribute.Value == "Value1");
    Assert.Contains(apiKey.CustomAttributes, customAttribute => customAttribute.Key == "Key2" && customAttribute.Value == "Value2");

    Assert.Null(apiKey.Realm);

    Assert.NotNull(apiKey.XApiKey);
    await AssertApiKeySecretAsync(apiKey.XApiKey);
  }

  [Fact(DisplayName = "CreateAsync: it should create a new realm API key.")]
  public async Task CreateAsync_it_should_create_a_new_realm_Api_key()
  {
    CreateApiKeyPayload payload = new()
    {
      Realm = $"  {_realm.UniqueSlug}  ",
      Title = "Default",
      Roles = new string[]
      {
        $"  {_readUsers.Id.ToGuid().ToString().ToUpper()}  ",
        $"  {_writeUsers.UniqueName.ToUpper()}  "
      }
    };

    ApiKey? apiKey = await _apiKeyService.CreateAsync(payload);
    Assert.NotNull(apiKey);

    Assert.NotEqual(Guid.Empty, apiKey.Id);
    Assert.Equal(Actor, apiKey.CreatedBy);
    AssertIsNear(apiKey.CreatedOn);
    Assert.Equal(apiKey.CreatedBy, apiKey.UpdatedBy);
    Assert.Equal(apiKey.CreatedBy, apiKey.UpdatedBy);
    Assert.True(apiKey.Version >= 1);

    Assert.Equal(payload.Title, apiKey.Title);

    Assert.Equal(2, apiKey.Roles.Count());
    Assert.Contains(apiKey.Roles, role => role.Id == _readUsers.Id.ToGuid());
    Assert.Contains(apiKey.Roles, role => role.Id == _writeUsers.Id.ToGuid());

    Assert.NotNull(apiKey.Realm);
    Assert.Equal(_realm.Id.ToGuid(), apiKey.Realm.Id);

    Assert.NotNull(apiKey.XApiKey);
    await AssertApiKeySecretAsync(apiKey.XApiKey);
  }

  [Fact(DisplayName = "CreateAsync: it should throw RolesNotFoundException when some roles could not be found.")]
  public async Task CreateAsync_it_should_throw_RolesNotFoundException_when_some_roles_could_not_be_found()
  {
    CreateApiKeyPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      Title = "Default",
      Roles = new string[] { _readUsers.UniqueName, "read_realms", "read_roles" }
    };

    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await _apiKeyService.CreateAsync(payload));
    Assert.Equal(new[] { "read_realms", "read_roles" }, exception.MissingRoles);
    Assert.Equal(nameof(payload.Roles), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when the realm could not be found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_the_realm_could_not_be_found()
  {
    CreateApiKeyPayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _apiKeyService.CreateAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the API key.")]
  public async Task DeleteAsync_it_should_delete_the_Api_key()
  {
    ApiKey? apiKey = await _apiKeyService.DeleteAsync(_apiKey.Id.ToGuid());

    Assert.NotNull(apiKey);
    Assert.Equal(_apiKey.Id.ToGuid(), apiKey.Id);

    Assert.Null(await PortalContext.ApiKeys.SingleOrDefaultAsync(x => x.AggregateId == _apiKey.Id.Value));
  }

  [Fact(DisplayName = "DeleteAsync: it should return null when the API key is not found.")]
  public async Task DeleteAsync_it_should_return_null_when_the_Api_key_is_not_found()
  {
    Assert.Null(await _apiKeyService.DeleteAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the API key is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_Api_key_is_not_found()
  {
    Assert.Null(await _apiKeyService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return the API key found by ID.")]
  public async Task ReadAsync_it_should_return_the_Api_key_found_by_Id()
  {
    ApiKey? apiKey = await _apiKeyService.ReadAsync(_apiKey.Id.ToGuid());
    Assert.NotNull(apiKey);
    Assert.Equal(_apiKey.Id.ToGuid(), apiKey.Id);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the API key.")]
  public async Task ReplaceAsync_it_should_replace_the_Api_key()
  {
    long version = _apiKey.Version;

    RoleAggregate readRealms = new(_realm.UniqueNameSettings, "read_realms", _realm.Id.Value);
    RoleAggregate readRoles = new(_realm.UniqueNameSettings, "read_roles", _realm.Id.Value);
    _apiKey.AddRole(readRealms);
    _apiKey.SetCustomAttribute("Key3", "Value3");
    await AggregateRepository.SaveAsync(new AggregateRoot[] { readRealms, readRoles, _apiKey });

    ReplaceApiKeyPayload payload = new()
    {
      Title = $"  {_apiKey.Title}2  ",
      Description = "    ",
      ExpiresOn = _apiKey.ExpiresOn?.AddMonths(-6),
      CustomAttributes = new CustomAttribute[]
      {
        new("Key2", "value2"),
        new("Key4", "Value4")
      },
      Roles = new string[] { "read_users", "read_roles" }
    };

    ApiKey? apiKey = await _apiKeyService.ReplaceAsync(_apiKey.Id.ToGuid(), payload, version);
    Assert.NotNull(apiKey);

    Assert.Equal(_apiKey.Id.ToGuid(), apiKey.Id);
    Assert.Equal(Guid.Empty, apiKey.CreatedBy.Id);
    AssertEqual(_apiKey.CreatedOn, apiKey.CreatedOn);
    Assert.Equal(Actor, apiKey.UpdatedBy);
    AssertIsNear(apiKey.UpdatedOn);
    Assert.True(apiKey.Version > version);

    Assert.Equal(payload.Title.Trim(), apiKey.Title);
    Assert.Equal(payload.Description?.CleanTrim(), apiKey.Description);
    AssertEqual(payload.ExpiresOn, apiKey.ExpiresOn);

    Assert.Equal(3, apiKey.CustomAttributes.Count());
    Assert.Contains(apiKey.CustomAttributes, customAttribute => customAttribute.Key == "Key2" && customAttribute.Value == "value2");
    Assert.Contains(apiKey.CustomAttributes, customAttribute => customAttribute.Key == "Key3" && customAttribute.Value == "Value3");
    Assert.Contains(apiKey.CustomAttributes, customAttribute => customAttribute.Key == "Key4" && customAttribute.Value == "Value4");

    Assert.Equal(3, apiKey.Roles.Count());
    Assert.Contains(apiKey.Roles, role => role.UniqueName == _readUsers.UniqueName);
    Assert.Contains(apiKey.Roles, role => role.UniqueName == readRealms.UniqueName);
    Assert.Contains(apiKey.Roles, role => role.UniqueName == readRoles.UniqueName);
  }

  [Fact(DisplayName = "ReplaceAsync: it should return null when the API key is not found.")]
  public async Task ReplaceAsync_it_should_return_null_when_the_Api_key_is_not_found()
  {
    ReplaceApiKeyPayload payload = new();
    Assert.Null(await _apiKeyService.ReplaceAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw RolesNotFoundException when some roles could not be found.")]
  public async Task ReplaceAsync_it_should_throw_RolesNotFoundException_when_some_roles_could_not_be_found()
  {
    ReplaceApiKeyPayload payload = new()
    {
      Title = _apiKey.Title,
      Roles = new string[] { "read_users", "read_roles", "read_realms" }
    };

    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await _apiKeyService.ReplaceAsync(_apiKey.Id.ToGuid(), payload));
    Assert.Equal(new[] { "read_roles", "read_realms" }, exception.MissingRoles);
    Assert.Equal(nameof(payload.Roles), exception.PropertyName);
  }

  private async Task AssertApiKeySecretAsync(string value)
  {
    XApiKey xApiKey = XApiKey.Decode(value);
    ApiKeyEntity? entity = await PortalContext.ApiKeys.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == xApiKey.Id.Value);
    Assert.NotNull(entity);

    Assert.NotNull(entity.Secret);
    Password secret = PasswordService.Decode(entity.Secret);
    Assert.True(secret.IsMatch(Convert.ToBase64String(xApiKey.Secret)));
  }
}
