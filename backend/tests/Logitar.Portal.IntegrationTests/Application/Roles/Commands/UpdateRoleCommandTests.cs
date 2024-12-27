using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Roles;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.Roles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateRoleCommandTests : IntegrationTests
{
  private readonly IRoleRepository _roleRepository;

  private readonly Role _role;

  public UpdateRoleCommandTests() : base()
  {
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();

    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    UniqueName uniqueName = new(uniqueNameSettings, "admin");
    _role = new(uniqueName);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.Roles.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _roleRepository.SaveAsync(_role);
  }

  [Fact(DisplayName = "It should return null when the role cannot be found.")]
  public async Task It_should_return_null_when_the_role_cannot_be_found()
  {
    UpdateRolePayload payload = new();
    UpdateRoleCommand command = new(Guid.NewGuid(), payload);
    RoleModel? role = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(role);
  }

  [Fact(DisplayName = "It should return null when the role is in another tenant.")]
  public async Task It_should_return_null_when_the_role_is_in_another_tenant()
  {
    SetRealm();

    UpdateRolePayload payload = new();
    UpdateRoleCommand command = new(_role.EntityId.ToGuid(), payload);
    RoleModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    Role role = new(new UniqueName(new ReadOnlyUniqueNameSettings(), "guest"));
    await _roleRepository.SaveAsync(role);

    UpdateRolePayload payload = new()
    {
      UniqueName = "AdmIn"
    };
    UpdateRoleCommand command = new(role.EntityId.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateRolePayload payload = new()
    {
      UniqueName = "/!\\"
    };
    UpdateRoleCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("UniqueName", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should update an existing role.")]
  public async Task It_should_update_an_existing_role()
  {
    _role.SetCustomAttribute(new Identifier("manage_users"), bool.TrueString);
    _role.SetCustomAttribute(new Identifier("manage_realms"), bool.FalseString);
    _role.SetCustomAttribute(new Identifier("configuration"), bool.FalseString);
    _role.Update();
    await _roleRepository.SaveAsync(_role);

    UpdateRolePayload payload = new()
    {
      DisplayName = new ChangeModel<string>("  Administrator  "),
      Description = new ChangeModel<string>("  ")
    };
    payload.CustomAttributes.Add(new("manage_roles", bool.TrueString));
    payload.CustomAttributes.Add(new("manage_realms", bool.TrueString));
    payload.CustomAttributes.Add(new("configuration", value: null));
    UpdateRoleCommand command = new(_role.EntityId.ToGuid(), payload);
    RoleModel? role = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(role);

    Assert.Equal(_role.UniqueName.Value, role.UniqueName);
    Assert.NotNull(payload.DisplayName.Value);
    Assert.Equal(payload.DisplayName.Value.Trim(), role.DisplayName);
    Assert.Null(role.Description);
    Assert.Null(role.Realm);

    Assert.Equal(3, role.CustomAttributes.Count);
    Assert.Contains(role.CustomAttributes, c => c.Key == "manage_users" && c.Value == bool.TrueString);
    Assert.Contains(role.CustomAttributes, c => c.Key == "manage_realms" && c.Value == bool.TrueString);
    Assert.Contains(role.CustomAttributes, c => c.Key == "manage_roles" && c.Value == bool.TrueString);
  }
}
