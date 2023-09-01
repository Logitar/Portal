using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class RoleServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IRoleService _roleService;

  private readonly RealmAggregate _realm;
  private readonly RoleAggregate _role;

  public RoleServiceTests() : base()
  {
    _roleService = ServiceProvider.GetRequiredService<IRoleService>();

    _realm = new("logitar");

    _role = new(_realm.UniqueNameSettings, "guest", _realm.Id.Value)
    {
      DisplayName = "Guest"
    };
    _role.SetCustomAttribute("read_roles", bool.TrueString);
    _role.SetCustomAttribute("read_users", bool.TrueString);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _role });
  }

  [Fact(DisplayName = "CreateAsync: it should create a role.")]
  public async Task CreateAsync_it_should_create_a_role()
  {
    CreateRolePayload payload = new()
    {
      Realm = $"  {_realm.UniqueSlug.ToUpper()}  ",
      UniqueName = "  admin  ",
      DisplayName = "  Administrator  ",
      Description = "  This is the administrator role.  ",
      CustomAttributes = new CustomAttribute[]
      {
        new("  Key1  ", "  Value1  "),
        new("Key2", "Value2")
      }
    };

    Role? role = await _roleService.CreateAsync(payload);
    Assert.NotNull(role);

    Assert.NotEqual(Guid.Empty, role.Id);
    Assert.Equal(Actor, role.CreatedBy);
    AssertIsNear(role.CreatedOn);
    Assert.Equal(Actor, role.UpdatedBy);
    AssertIsNear(role.UpdatedOn);
    Assert.True(role.Version >= 1);

    Assert.Equal(payload.UniqueName.Trim(), role.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), role.DisplayName);
    Assert.Equal(payload.Description.Trim(), role.Description);

    Assert.Equal(2, role.CustomAttributes.Count());
    Assert.Contains(role.CustomAttributes, customAttribute => customAttribute.Key == "Key1" && customAttribute.Value == "Value1");
    Assert.Contains(role.CustomAttributes, customAttribute => customAttribute.Key == "Key2" && customAttribute.Value == "Value2");

    Assert.NotNull(role.Realm);
    Assert.Equal(_realm.Id.ToGuid(), role.Realm.Id);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when the realm is not found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_the_realm_is_not_found()
  {
    CreateRolePayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _roleService.CreateAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task CreateAsync_it_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    CreateRolePayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      UniqueName = _role.UniqueName
    };

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<RoleAggregate>>(async () => await _roleService.CreateAsync(payload));
    Assert.Equal(_realm.Id.Value, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the role.")]
  public async Task DeleteAsync_it_should_delete_the_role()
  {
    RoleAggregate other = new(_realm.UniqueNameSettings, "other_role", _realm.Id.Value);

    Password secret = PasswordService.Generate(_realm.PasswordSettings, ApiKeyAggregate.SecretLength, out _);
    ApiKeyAggregate apiKey = new("Default", secret, _realm.Id.Value);
    apiKey.AddRole(other);
    apiKey.AddRole(_role);

    UserAggregate user = new(_realm.UniqueNameSettings, Faker.Person.Website, _realm.Id.Value);
    user.AddRole(other);
    user.AddRole(_role);

    await AggregateRepository.SaveAsync(new AggregateRoot[] { other, apiKey, user });

    Role? role = await _roleService.DeleteAsync(_role.Id.ToGuid());

    Assert.NotNull(role);
    Assert.Equal(_role.Id.ToGuid(), role.Id);

    Assert.Null(await PortalContext.Roles.SingleOrDefaultAsync(x => x.AggregateId == _role.Id.Value));

    ApiKeyEntity? apiKeyEntity = await PortalContext.ApiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == apiKey.Id.Value);
    Assert.NotNull(apiKeyEntity);
    Assert.Contains(apiKeyEntity.Roles, role => role.AggregateId == other.Id.Value);
    Assert.DoesNotContain(apiKeyEntity.Roles, role => role.AggregateId == _role.Id.Value);

    UserEntity? userEntity = await PortalContext.Users.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == user.Id.Value);
    Assert.NotNull(userEntity);
    Assert.Contains(userEntity.Roles, role => role.AggregateId == other.Id.Value);
    Assert.DoesNotContain(userEntity.Roles, role => role.AggregateId == _role.Id.Value);
  }

  [Fact(DisplayName = "DeleteAsync: it should return null when the role is not found.")]
  public async Task DeleteAsync_it_should_return_null_when_the_role_is_not_found()
  {
    Assert.Null(await _roleService.DeleteAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the role is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_role_is_not_found()
  {
    Assert.Null(await _roleService.ReadAsync(Guid.Empty, _realm.UniqueSlug, $"{_role.UniqueName}2"));
  }

  [Fact(DisplayName = "ReadAsync: it should return the role found by unique name.")]
  public async Task ReadAsync_it_should_return_the_role_found_by_unique_name()
  {
    Role? role = await _roleService.ReadAsync(realm: $" {_realm.Id.ToGuid()} ", uniqueName: $" {_role.UniqueName} ");
    Assert.NotNull(role);
    Assert.Equal(_role.Id.ToGuid(), role.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return the role found by ID.")]
  public async Task ReadAsync_it_should_return_the_role_found_by_Id()
  {
    Role? role = await _roleService.ReadAsync(_role.Id.ToGuid());
    Assert.NotNull(role);
    Assert.Equal(_role.Id.ToGuid(), role.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should throw TooManyResultsException when many roles have been found.")]
  public async Task ReadAsync_it_should_throw_TooManyResultsException_when_many_roles_have_been_found()
  {
    RoleAggregate role = new(new ReadOnlyUniqueNameSettings(), "other_role");
    await AggregateRepository.SaveAsync(role);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<Role>>(
      async () => await _roleService.ReadAsync(role.Id.ToGuid(), _realm.UniqueSlug, _role.UniqueName)
    );
    Assert.Equal(1, exception.Expected);
    Assert.Equal(2, exception.Actual);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the role.")]
  public async Task ReplaceAsync_it_should_replace_the_role()
  {
    long version = _role.Version;

    _role.SetCustomAttribute("read_realms", bool.TrueString);
    await AggregateRepository.SaveAsync(_role);

    ReplaceRolePayload payload = new()
    {
      UniqueName = $"  {_role.UniqueName}2  ",
      DisplayName = "  Guest 2  ",
      Description = "  This is the role for guest users.  ",
      CustomAttributes = new CustomAttribute[]
      {
        new("manage_roles", bool.TrueString),
        new("read_users", bool.FalseString)
      }
    };

    Role? role = await _roleService.ReplaceAsync(_role.Id.ToGuid(), payload, version);

    Assert.NotNull(role);

    Assert.Equal(_role.Id.ToGuid(), role.Id);
    Assert.Equal(Guid.Empty, role.CreatedBy.Id);
    AssertEqual(_role.CreatedOn, role.CreatedOn);
    Assert.Equal(Actor, role.UpdatedBy);
    AssertIsNear(role.UpdatedOn);
    Assert.True(role.Version > 1);

    Assert.Equal(payload.UniqueName.Trim(), role.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), role.DisplayName);
    Assert.Equal(payload.Description.Trim(), role.Description);

    Assert.Equal(3, role.CustomAttributes.Count());
    Assert.Contains(role.CustomAttributes, customAttribute => customAttribute.Key == "manage_roles"
      && customAttribute.Value == bool.TrueString);
    Assert.Contains(role.CustomAttributes, customAttribute => customAttribute.Key == "read_users"
      && customAttribute.Value == bool.FalseString);
    Assert.Contains(role.CustomAttributes, customAttribute => customAttribute.Key == "read_realms"
      && customAttribute.Value == bool.TrueString);
  }

  [Fact(DisplayName = "ReplaceAsync: it should return null when the role is not found.")]
  public async Task ReplaceAsync_it_should_return_null_when_the_role_is_not_found()
  {
    ReplaceRolePayload payload = new();
    Assert.Null(await _roleService.ReplaceAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "ReplaceAsync: it should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task ReplaceAsync_it_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    RoleAggregate role = new(_realm.UniqueNameSettings, $"{_role.UniqueName}2", _realm.Id.Value);
    await AggregateRepository.SaveAsync(role);

    ReplaceRolePayload payload = new()
    {
      UniqueName = $" {role.UniqueName} "
    };

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<RoleAggregate>>(
      async () => await _roleService.ReplaceAsync(_role.Id.ToGuid(), payload)
    );
    Assert.Equal(role.TenantId, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchRolesPayload payload = new()
    {
      IdIn = new[] { Guid.Empty }
    };

    SearchResults<Role> results = await _roleService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    RoleAggregate notInRealm = new(new ReadOnlyUniqueNameSettings(), "read_users");
    RoleAggregate writeUsers = new(_realm.UniqueNameSettings, "write_users", _realm.Id.Value);
    RoleAggregate idNotIn = new(_realm.UniqueNameSettings, "read_users", _realm.Id.Value);
    RoleAggregate role1 = new(_realm.UniqueNameSettings, "read_roles", _realm.Id.Value)
    {
      DisplayName = "Read Roles"
    };
    RoleAggregate role2 = new(_realm.UniqueNameSettings, "read_api_keys", _realm.Id.Value)
    {
      DisplayName = "Read API Keys"
    };
    RoleAggregate role3 = new(_realm.UniqueNameSettings, "read_realms", _realm.Id.Value)
    {
      DisplayName = "Read Realms"
    };
    RoleAggregate role4 = new(_realm.UniqueNameSettings, "read_configuration", _realm.Id.Value)
    {
      DisplayName = "Read Configuration"
    };
    await AggregateRepository.SaveAsync(new[] { notInRealm, writeUsers, idNotIn, role1, role2, role3, role4 });

    RoleAggregate[] roles = new[] { role1, role2, role3, role4 }
      .OrderBy(x => x.DisplayName).Skip(1).Take(2).ToArray();

    HashSet<Guid> ids = (await PortalContext.Roles.AsNoTracking().ToArrayAsync())
      .Select(role => new AggregateId(role.AggregateId).ToGuid()).ToHashSet();
    ids.Remove(idNotIn.Id.ToGuid());

    SearchRolesPayload payload = new()
    {
      Search = new TextSearch
      {
        Operator = SearchOperator.Or,
        Terms = new SearchTerm[]
        {
          new("r__d%"),
          new(Guid.NewGuid().ToString())
        }
      },
      IdIn = ids,
      Realm = $" {_realm.UniqueSlug.ToUpper()} ",
      Sort = new RoleSortOption[]
      {
        new(RoleSort.DisplayName)
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Role> results = await _roleService.SearchAsync(payload);

    Assert.Equal(roles.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < roles.Length; i++)
    {
      Assert.Equal(roles[i].Id.ToGuid(), results.Results.ElementAt(i).Id);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should return null when the role is not found.")]
  public async Task UpdateAsync_it_should_return_null_when_the_role_is_not_found()
  {
    UpdateRolePayload payload = new();
    Assert.Null(await _roleService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task UpdateAsync_it_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    RoleAggregate role = new(_realm.UniqueNameSettings, $"{_role.UniqueName}2", _realm.Id.Value);
    await AggregateRepository.SaveAsync(role);

    UpdateRolePayload payload = new()
    {
      UniqueName = $" {role.UniqueName} "
    };

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<RoleAggregate>>(
      async () => await _roleService.UpdateAsync(_role.Id.ToGuid(), payload)
    );
    Assert.Equal(role.TenantId, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should update the role.")]
  public async Task UpdateAsync_it_should_update_the_role()
  {
    UpdateRolePayload payload = new()
    {
      UniqueName = $"  {_role.UniqueName}2  ",
      DisplayName = new Modification<string>("  Guest 2  "),
      Description = new Modification<string>("  This is the role for guest users.  "),
      CustomAttributes = new CustomAttributeModification[]
      {
        new("manage_roles", bool.TrueString),
        new("read_users", bool.FalseString),
        new("read_roles", null)
      }
    };

    Role? role = await _roleService.UpdateAsync(_role.Id.ToGuid(), payload);

    Assert.NotNull(role);

    Assert.Equal(_role.Id.ToGuid(), role.Id);
    Assert.Equal(Guid.Empty, role.CreatedBy.Id);
    AssertEqual(_role.CreatedOn, role.CreatedOn);
    Assert.Equal(Actor, role.UpdatedBy);
    AssertIsNear(role.UpdatedOn);
    Assert.True(role.Version > 1);

    Assert.Equal(payload.UniqueName.Trim(), role.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), role.DisplayName);
    Assert.Equal(payload.Description.Value?.Trim(), role.Description);

    Assert.Equal(2, role.CustomAttributes.Count());
    Assert.Contains(role.CustomAttributes, customAttribute => customAttribute.Key == "manage_roles"
      && customAttribute.Value == bool.TrueString);
    Assert.Contains(role.CustomAttributes, customAttribute => customAttribute.Key == "read_users"
      && customAttribute.Value == bool.FalseString);
  }
}
