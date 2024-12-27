using Logitar.Data;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Roles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteRoleCommandTests : IntegrationTests
{
  private readonly IRoleRepository _roleRepository;
  private readonly IUserRepository _userRepository;

  private readonly Role _role;

  public DeleteRoleCommandTests() : base()
  {
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

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

  [Fact(DisplayName = "It should delete an existing role.")]
  public async Task It_should_delete_an_existing_role()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());
    user.AddRole(_role);
    await _userRepository.SaveAsync(user);

    DeleteRoleCommand command = new(_role.Id.ToGuid());
    RoleModel? role = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(role);
    Assert.Equal(command.Id, role.Id);

    user = Assert.Single(await _userRepository.LoadAsync());
    Assert.False(user.HasRole(_role));
  }

  [Fact(DisplayName = "It should return null when the role cannot be found.")]
  public async Task It_should_return_null_when_the_role_cannot_be_found()
  {
    DeleteRoleCommand command = new(Guid.NewGuid());
    RoleModel? role = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(role);
  }

  [Fact(DisplayName = "It should return null when the role is in another tenant.")]
  public async Task It_should_return_null_when_the_role_is_in_another_tenant()
  {
    SetRealm();

    DeleteRoleCommand command = new(_role.Id.ToGuid());
    RoleModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }
}
