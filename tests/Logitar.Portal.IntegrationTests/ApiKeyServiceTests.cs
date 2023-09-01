using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
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
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _readUsers, _writeUsers });
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
    Assert.Equal(ToUnixTimeMilliseconds(payload.ExpiresOn), ToUnixTimeMilliseconds(apiKey.ExpiresOn));

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

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when some roles could not be found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_some_roles_could_not_be_found()
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
