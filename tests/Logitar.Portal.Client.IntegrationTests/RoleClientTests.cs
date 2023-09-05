using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Client;

internal class RoleClientTests
{
  private const string Sut = "RoleClient";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly IRoleService _roleService;

  public RoleClientTests(TestContext context, Faker faker, IRoleService roleService)
  {
    _context = context;
    _faker = faker;
    _roleService = roleService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_roleService.CreateAsync)}";
      CreateRolePayload create = new()
      {
        Realm = _context.Realm.UniqueSlug,
        UniqueName = "admin"
      };
      Role? role = await _roleService.CreateAsync(create, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_roleService.ReplaceAsync)}";
      ReplaceRolePayload replace = new()
      {
        UniqueName = role.UniqueName,
        DisplayName = "Administrator"
      };
      role = await _roleService.ReplaceAsync(role.Id, replace, role.Version, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_roleService.UpdateAsync)}";
      UpdateRolePayload update = new()
      {
        CustomAttributes = new CustomAttributeModification[]
        {
          new("read_users", bool.TrueString),
          new("write_users", bool.TrueString)
        }
      };
      role = await _roleService.UpdateAsync(role.Id, update, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_roleService.DeleteAsync)}";
      Role delete = await _roleService.CreateAsync(new CreateRolePayload
      {
        Realm = _context.Realm.Id.ToString(),
        UniqueName = $"{role.UniqueName}2"
      }, cancellationToken);
      delete = await _roleService.DeleteAsync(delete.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_roleService.SearchAsync)}";
      SearchRolesPayload search = new()
      {
        Realm = _context.Realm.UniqueSlug,
        IdIn = new Guid[] { role.Id }
      };
      SearchResults<Role> results = await _roleService.SearchAsync(search, cancellationToken);
      role = results.Results.Single();
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_roleService.ReadAsync)}:null";
      Role? result = await _roleService.ReadAsync(Guid.Empty, cancellationToken: cancellationToken);
      if (result != null)
      {
        throw new InvalidOperationException("The realm should be null.");
      }
      _context.Succeed(name);
      name = $"{Sut}.{nameof(_roleService.ReadAsync)}:Id";
      role = await _roleService.ReadAsync(role.Id, cancellationToken: cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);
      name = $"{Sut}.{nameof(_roleService.ReadAsync)}:UniqueSlug";
      role = await _roleService.ReadAsync(realm: _context.Realm.Id.ToString(), uniqueName: role.UniqueName, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }
}
