﻿using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Roles;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.Roles.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadRoleQueryTests : IntegrationTests
{
  private readonly IRoleRepository _roleRepository;

  private readonly Role _role;

  public ReadRoleQueryTests() : base()
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
    SetRealm();

    ReadRoleQuery query = new(_role.EntityId.ToGuid(), UniqueName: null);
    RoleModel? role = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(role);
  }

  [Fact(DisplayName = "It should return the role found by ID.")]
  public async Task It_should_return_the_role_found_by_Id()
  {
    ReadRoleQuery query = new(_role.EntityId.ToGuid(), _role.UniqueName.Value);
    RoleModel? role = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(role);
    Assert.Equal(_role.EntityId.ToGuid(), role.Id);
  }

  [Fact(DisplayName = "It should return the role found by unique name.")]
  public async Task It_should_return_the_role_found_by_unique_name()
  {
    ReadRoleQuery query = new(Id: null, _role.UniqueName.Value);
    RoleModel? role = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(role);
    Assert.Equal(_role.EntityId.ToGuid(), role.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when there are too many results.")]
  public async Task It_should_throw_TooManyResultsException_when_there_are_too_many_results()
  {
    Role role = new(new UniqueName(new ReadOnlyUniqueNameSettings(), "guest"));
    await _roleRepository.SaveAsync(role);

    ReadRoleQuery query = new(_role.EntityId.ToGuid(), "  GueST  ");
    var exception = await Assert.ThrowsAsync<TooManyResultsException<RoleModel>>(async () => await ActivityPipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
