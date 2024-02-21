using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Roles.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceRoleCommandTests : IntegrationTests
{
  private readonly IRoleRepository _roleRepository;

  private readonly RoleAggregate _role;

  public ReplaceRoleCommandTests() : base()
  {
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();

    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    UniqueNameUnit uniqueName = new(uniqueNameSettings, "admin");
    _role = new(uniqueName);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.Roles.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _roleRepository.SaveAsync(_role);
  }

  [Fact(DisplayName = "It should replace an existing role.")]
  public async Task It_should_replace_an_existing_role()
  {
    _role.SetCustomAttribute("manage_users", bool.TrueString);
    _role.SetCustomAttribute("configuration", bool.FalseString);
    _role.Update();
    await _roleRepository.SaveAsync(_role);
    long version = _role.Version;

    _role.SetCustomAttribute("manage_roles", bool.FalseString);
    _role.SetCustomAttribute("manage_realms", bool.FalseString);
    _role.Update();
    await _roleRepository.SaveAsync(_role);

    ReplaceRolePayload payload = new("guest")
    {
      DisplayName = "  Guest  ",
      Description = "         "
    };
    payload.CustomAttributes.Add(new("manage_users", bool.FalseString));
    payload.CustomAttributes.Add(new("manage_roles", bool.TrueString));
    ReplaceRoleCommand command = new(_role.Id.ToGuid(), payload, version);
    Role? role = await Mediator.Send(command);
    Assert.NotNull(role);

    Assert.Equal(payload.UniqueName, role.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), role.DisplayName);
    Assert.Null(role.Description);

    Assert.Equal(3, role.CustomAttributes.Count);
    Assert.Contains(role.CustomAttributes, c => c.Key == "manage_users" && c.Value == bool.FalseString);
    Assert.Contains(role.CustomAttributes, c => c.Key == "manage_roles" && c.Value == bool.TrueString);
    Assert.Contains(role.CustomAttributes, c => c.Key == "manage_realms" && c.Value == bool.FalseString);
  }

  [Fact(DisplayName = "It should return null when the role cannot be found.")]
  public async Task It_should_return_null_when_the_role_cannot_be_found()
  {
    ReplaceRolePayload payload = new("admin");
    ReplaceRoleCommand command = new(Guid.NewGuid(), payload, Version: null);
    Role? role = await Mediator.Send(command);
    Assert.Null(role);
  }

  [Fact(DisplayName = "It should return null when the role is in another tenant.")]
  public async Task It_should_return_null_when_the_role_is_in_another_tenant()
  {
    SetRealm();

    ReplaceRolePayload payload = new("admin");
    ReplaceRoleCommand command = new(_role.Id.ToGuid(), payload, Version: null);
    Role? result = await Mediator.Send(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    RoleAggregate role = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), "guest"));
    await _roleRepository.SaveAsync(role);

    ReplaceRolePayload payload = new("AdmIn");
    ReplaceRoleCommand command = new(role.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<RoleAggregate>>(async () => await Mediator.Send(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName.Value);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceRolePayload payload = new("/!\\");
    ReplaceRoleCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("UniqueName", exception.Errors.Single().PropertyName);
  }
}
