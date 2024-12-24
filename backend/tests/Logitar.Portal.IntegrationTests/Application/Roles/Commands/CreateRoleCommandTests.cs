using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Roles;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.Roles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateRoleCommandTests : IntegrationTests
{
  private readonly IRoleRepository _roleRepository;

  public CreateRoleCommandTests() : base()
  {
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
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
  }

  [Fact(DisplayName = "It should create a new role.")]
  public async Task It_should_create_a_new_role()
  {
    CreateRolePayload payload = new("admin")
    {
      DisplayName = "Administrator",
      Description = "  This is the administration role.  "
    };
    payload.CustomAttributes.Add(new("root", bool.TrueString));
    CreateRoleCommand command = new(payload);
    RoleModel role = await ActivityPipeline.ExecuteAsync(command);

    Assert.Equal(payload.UniqueName, role.UniqueName);
    Assert.Equal(payload.DisplayName, role.DisplayName);
    Assert.Equal(payload.Description.Trim(), role.Description);
    Assert.Equal(payload.CustomAttributes, role.CustomAttributes);
    Assert.Null(role.Realm);

    SetRealm();
    RoleModel other = await ActivityPipeline.ExecuteAsync(command);
    Assert.Same(Realm, other.Realm);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    SetRealm();

    Role role = new(new UniqueName(new ReadOnlyUniqueNameSettings(), "admin"), roleId: RoleId.NewId(TenantId));
    await _roleRepository.SaveAsync(role);

    CreateRolePayload payload = new(role.UniqueName.Value);
    CreateRoleCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(TenantId.Value, exception.TenantId);
    Assert.Equal(role.UniqueName.Value, exception.UniqueName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateRolePayload payload = new(uniqueName: "");
    CreateRoleCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("UniqueName", exception.Errors.Single().PropertyName);
  }
}
