using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceApiKeyCommandTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly IRoleRepository _roleRepository;

  public ReplaceApiKeyCommandTests() : base()
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
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should replace an existing API key.")]
  public async Task It_should_replace_an_existing_Api_key()
  {
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    RoleAggregate admin = new(new UniqueNameUnit(uniqueNameSettings, "admin"));
    RoleAggregate manageUsers = new(new UniqueNameUnit(uniqueNameSettings, "manage_users"));
    RoleAggregate sendMessages = new(new UniqueNameUnit(uniqueNameSettings, "sendMessages"));
    await _roleRepository.SaveAsync([admin, manageUsers, sendMessages]);

    ApiKeyAggregate apiKey = await CreateApiKeyAsync();

    apiKey.SetCustomAttribute("Confidentiality", "Private");
    apiKey.SetCustomAttribute("SubSystem", "tests");
    apiKey.Update();
    apiKey.AddRole(admin);
    await _apiKeyRepository.SaveAsync(apiKey);
    long version = apiKey.Version;

    apiKey.SetCustomAttribute("Confidentiality", "Public");
    string userId = Guid.NewGuid().ToString();
    apiKey.SetCustomAttribute("UserId", userId);
    apiKey.Update();
    apiKey.AddRole(manageUsers);
    apiKey.RemoveRole(admin);
    await _apiKeyRepository.SaveAsync(apiKey);

    ReplaceApiKeyPayload payload = new("Default API Key")
    {
      Description = "  This is the default API key.  ",
      ExpiresOn = DateTime.Now.AddDays(90)
    };
    payload.CustomAttributes.Add(new("SubSystem", "user_management"));
    payload.CustomAttributes.Add(new("AccessControl", bool.FalseString));
    payload.Roles.Add(admin.UniqueName.Value);
    payload.Roles.Add(sendMessages.UniqueName.Value);
    ReplaceApiKeyCommand command = new(apiKey.Id.AggregateId.ToGuid(), payload, version);
    ApiKey? result = await Mediator.Send(command);
    Assert.NotNull(result);

    Assert.Equal(payload.DisplayName, result.DisplayName);
    Assert.Equal(payload.Description.Trim(), result.Description);
    Assert.Equal(payload.ExpiresOn?.ToUniversalTime(), result.ExpiresOn);

    Assert.Equal(3, result.CustomAttributes.Count);
    Assert.Contains(result.CustomAttributes, c => c.Key == "AccessControl" && c.Value == bool.FalseString);
    Assert.Contains(result.CustomAttributes, c => c.Key == "SubSystem" && c.Value == "user_management");
    Assert.Contains(result.CustomAttributes, c => c.Key == "UserId" && c.Value == userId);

    Assert.Equal(2, result.Roles.Count);
    Assert.Contains(result.Roles, r => r.Id == manageUsers.Id.AggregateId.ToGuid());
    Assert.Contains(result.Roles, r => r.Id == sendMessages.Id.AggregateId.ToGuid());
  }

  [Fact(DisplayName = "It should return null when the API key cannot be found.")]
  public async Task It_should_return_null_when_the_Api_key_cannot_be_found()
  {
    ReplaceApiKeyPayload payload = new("admin");
    ReplaceApiKeyCommand command = new(Guid.NewGuid(), payload, Version: null);
    ApiKey? apiKey = await Mediator.Send(command);
    Assert.Null(apiKey);
  }

  [Fact(DisplayName = "It should return null when the API key is in another tenant.")]
  public async Task It_should_return_null_when_the_Api_key_is_in_another_tenant()
  {
    ApiKeyAggregate apiKey = await CreateApiKeyAsync();

    SetRealm();

    ReplaceApiKeyPayload payload = new("admin");
    ReplaceApiKeyCommand command = new(apiKey.Id.AggregateId.ToGuid(), payload, Version: null);
    ApiKey? result = await Mediator.Send(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw RolesNotFoundException when some roles cannot be found.")]
  public async Task It_should_throw_RolesNotFoundException_when_some_roles_cannot_be_found()
  {
    ApiKeyAggregate apiKey = await CreateApiKeyAsync();

    ReplaceApiKeyPayload payload = new(apiKey.DisplayName.Value);
    payload.Roles.Add("admin");
    ReplaceApiKeyCommand command = new(apiKey.Id.AggregateId.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.Roles, exception.Roles);
    Assert.Equal("Roles", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceApiKeyPayload payload = new(displayName: string.Empty);
    ReplaceApiKeyCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("DisplayName", exception.Errors.Single().PropertyName);
  }

  private async Task<ApiKeyAggregate> CreateApiKeyAsync()
  {
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out _);
    ApiKeyAggregate apiKey = new(new DisplayNameUnit("Default"), secret);

    await _apiKeyRepository.SaveAsync(apiKey);

    return apiKey;
  }
}
