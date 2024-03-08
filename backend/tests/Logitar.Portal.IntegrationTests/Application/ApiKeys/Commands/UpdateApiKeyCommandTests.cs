using Logitar.Data;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateApiKeyCommandTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly IRoleRepository _roleRepository;

  public UpdateApiKeyCommandTests() : base()
  {
    _apiKeyRepository = ServiceProvider.GetRequiredService<IApiKeyRepository>();
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.ApiKeys.Table, IdentityDb.Roles.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should return null when the API key cannot be found.")]
  public async Task It_should_return_null_when_the_Api_key_cannot_be_found()
  {
    UpdateApiKeyPayload payload = new();
    UpdateApiKeyCommand command = new(Guid.NewGuid(), payload);
    ApiKey? apiKey = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(apiKey);
  }

  [Fact(DisplayName = "It should return null when the API key is in another tenant.")]
  public async Task It_should_return_null_when_the_Api_key_is_in_another_tenant()
  {
    ApiKeyAggregate apiKey = await CreateApiKeyAsync();

    SetRealm();

    UpdateApiKeyPayload payload = new();
    UpdateApiKeyCommand command = new(apiKey.Id.ToGuid(), payload);
    ApiKey? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw RolesNotFoundException when some roles cannot be found.")]
  public async Task It_should_throw_RolesNotFoundException_when_some_roles_cannot_be_found()
  {
    ApiKeyAggregate apiKey = await CreateApiKeyAsync();

    UpdateApiKeyPayload payload = new();
    payload.Roles.Add(new RoleModification("admin"));
    UpdateApiKeyCommand command = new(apiKey.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Roles.Select(role => role.Role), exception.Roles);
    Assert.Equal("Roles", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateApiKeyPayload payload = new()
    {
      ExpiresOn = DateTime.Now.AddDays(-1)
    };
    UpdateApiKeyCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("ExpiresOn.Value", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should update an existing API key.")]
  public async Task It_should_update_an_existing_Api_key()
  {
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    RoleAggregate admin = new(new UniqueNameUnit(uniqueNameSettings, "admin"));
    RoleAggregate editor = new(new UniqueNameUnit(uniqueNameSettings, "editor"));
    RoleAggregate reviewer = new(new UniqueNameUnit(uniqueNameSettings, "reviewer"));
    await _roleRepository.SaveAsync([admin, editor, reviewer]);

    ApiKeyAggregate apiKey = await CreateApiKeyAsync();

    apiKey.SetCustomAttribute("UserId", Guid.NewGuid().ToString());
    apiKey.SetCustomAttribute("SubSystem", "tests");
    apiKey.Update();
    apiKey.AddRole(admin);
    apiKey.AddRole(reviewer);
    await _apiKeyRepository.SaveAsync(apiKey);

    UpdateApiKeyPayload payload = new()
    {
      DisplayName = "Default API Key",
      Description = new Modification<string>("  This is the default API key.  "),
      ExpiresOn = DateTime.Now.AddMonths(6)
    };
    payload.CustomAttributes.Add(new("Confidentiality", "Private"));
    payload.CustomAttributes.Add(new("SubSystem", nameof(IntegrationTests)));
    payload.CustomAttributes.Add(new("UserId", value: null));
    payload.Roles.Add(new(editor.UniqueName.Value, CollectionAction.Add));
    payload.Roles.Add(new(admin.UniqueName.Value, CollectionAction.Remove));
    UpdateApiKeyCommand command = new(apiKey.Id.ToGuid(), payload);
    ApiKey? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(result);

    Assert.Equal(payload.DisplayName, result.DisplayName);
    Assert.Equal(payload.Description.Value?.Trim(), result.Description);
    Assertions.Equal(payload.ExpiresOn, result.ExpiresOn, TimeSpan.FromSeconds(1));

    Assert.Equal(2, result.CustomAttributes.Count);
    Assert.Contains(result.CustomAttributes, c => c.Key == "Confidentiality" && c.Value == "Private");
    Assert.Contains(result.CustomAttributes, c => c.Key == "SubSystem" && c.Value == nameof(IntegrationTests));

    Assert.Equal(2, result.Roles.Count);
    Assert.Contains(result.Roles, r => r.Id == editor.Id.ToGuid());
    Assert.Contains(result.Roles, r => r.Id == reviewer.Id.ToGuid());
  }

  private async Task<ApiKeyAggregate> CreateApiKeyAsync()
  {
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out _);
    ApiKeyAggregate apiKey = new(new DisplayNameUnit("Default"), secret);

    await _apiKeyRepository.SaveAsync(apiKey);

    return apiKey;
  }
}
