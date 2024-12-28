using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Roles;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateApiKeyCommandTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly IRoleRepository _roleRepository;

  public CreateApiKeyCommandTests() : base()
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

  [Theory(DisplayName = "It should create a new API key.")]
  [InlineData(null)]
  [InlineData("546d56db-7364-4c48-a135-a5e0e64500c4")]
  public async Task It_should_create_a_new_Api_key(string? idValue)
  {
    SetRealm();

    Role role = new(new UniqueName(Realm.UniqueNameSettings, "manage_sales"), actorId: null, RoleId.NewId(TenantId));
    await _roleRepository.SaveAsync(role);

    CreateApiKeyPayload payload = new("Default")
    {
      Description = "  This is the default API key.  ",
      ExpiresOn = DateTime.Now.AddYears(1)
    };
    if (idValue != null)
    {
      payload.Id = Guid.Parse(idValue);
    }
    payload.CustomAttributes.Add(new("JobTitle", "Sales Manager"));
    payload.Roles.Add("  Manage_Sales  ");
    CreateApiKeyCommand command = new(payload);
    ApiKeyModel apiKey = await ActivityPipeline.ExecuteAsync(command);

    if (payload.Id.HasValue)
    {
      Assert.Equal(payload.Id.Value, apiKey.Id);
    }
    Assert.Equal(payload.DisplayName, apiKey.DisplayName);
    Assert.Equal(payload.Description.Trim(), apiKey.Description);
    Assertions.Equal(payload.ExpiresOn, apiKey.ExpiresOn, TimeSpan.FromSeconds(1));
    Assert.Equal(payload.CustomAttributes, apiKey.CustomAttributes);
    Assert.Same(Realm, apiKey.Realm);

    RoleModel apiKeyRole = Assert.Single(apiKey.Roles);
    Assert.Equal(role.EntityId.ToGuid(), apiKeyRole.Id);

    Assert.NotNull(apiKey.XApiKey);
    XApiKey xApiKey = XApiKey.Decode(TenantId, apiKey.XApiKey);
    Assert.Equal(apiKey.Id, xApiKey.Id.EntityId.ToGuid());
    Assert.Equal(XApiKey.SecretLength, Convert.FromBase64String(xApiKey.Secret).Length);
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already taken.")]
  public async Task It_should_throw_IdAlreadyUsedException_when_the_Id_is_already_taken()
  {
    Password secret = _passwordManager.GenerateBase64(256 / 8, out _);
    ApiKey apiKey = new(new DisplayName("Test"), secret);
    await _apiKeyRepository.SaveAsync(apiKey);

    CreateApiKeyPayload payload = new(apiKey.DisplayName.Value)
    {
      Id = apiKey.EntityId.ToGuid()
    };
    CreateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Id.Value, exception.Id);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw RolesNotFoundException when some roles cannot be found.")]
  public async Task It_should_throw_RolesNotFoundException_when_some_roles_cannot_be_found()
  {
    CreateApiKeyPayload payload = new("Default");
    payload.Roles.Add("admin");
    CreateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Roles, exception.Roles);
    Assert.Equal("Roles", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateApiKeyPayload payload = new(displayName: string.Empty)
    {
      Id = Guid.Empty
    };
    CreateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Id.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "DisplayName");
  }
}
