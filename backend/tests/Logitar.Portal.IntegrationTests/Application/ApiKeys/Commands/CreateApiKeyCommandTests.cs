using Logitar.Data;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Roles;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateApiKeyCommandTests : IntegrationTests
{
  private readonly IRoleRepository _roleRepository;

  public CreateApiKeyCommandTests() : base()
  {
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

  [Fact(DisplayName = "It should create a new API key.")]
  public async Task It_should_create_a_new_Api_key()
  {
    SetRealm();

    RoleAggregate role = new(new UniqueNameUnit(Realm.UniqueNameSettings, "manage_sales"), TenantId);
    await _roleRepository.SaveAsync(role);

    CreateApiKeyPayload payload = new("Default")
    {
      Description = "  This is the default API key.  ",
      ExpiresOn = DateTime.Now.AddYears(1)
    };
    payload.CustomAttributes.Add(new("JobTitle", "Sales Manager"));
    payload.Roles.Add("  Manage_Sales  ");
    CreateApiKeyCommand command = new(payload);
    ApiKey apiKey = await Mediator.Send(command);

    Assert.Equal(payload.DisplayName, apiKey.DisplayName);
    Assert.Equal(payload.Description.Trim(), apiKey.Description);
    Assertions.Equal(payload.ExpiresOn, apiKey.ExpiresOn, TimeSpan.FromSeconds(1));
    Assert.Equal(payload.CustomAttributes, apiKey.CustomAttributes);
    Assert.Same(Realm, apiKey.Realm);

    Role apiKeyRole = Assert.Single(apiKey.Roles);
    Assert.Equal(role.Id.ToGuid(), apiKeyRole.Id);

    Assert.NotNull(apiKey.XApiKey);
    XApiKey xApiKey = XApiKey.Decode(apiKey.XApiKey);
    Assert.Equal(apiKey.Id, xApiKey.Id.ToGuid());
    Assert.Equal(XApiKey.SecretLength, Convert.FromBase64String(xApiKey.Secret).Length);
  }

  [Fact(DisplayName = "It should throw RolesNotFoundException when some roles cannot be found.")]
  public async Task It_should_throw_RolesNotFoundException_when_some_roles_cannot_be_found()
  {
    CreateApiKeyPayload payload = new("Default");
    payload.Roles.Add("admin");
    CreateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<RolesNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.Roles, exception.Roles);
    Assert.Equal("Roles", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateApiKeyPayload payload = new(displayName: string.Empty);
    CreateApiKeyCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("DisplayName", exception.Errors.Single().PropertyName);
  }
}
