using Logitar.Identity.Core;
using Logitar.Identity.Core.Roles;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal record FindRolesQuery : IRequest<IReadOnlyCollection<FoundRole>>
{
  public TenantId? TenantId { get; }
  public IReadOnlyCollection<RoleModification> Roles { get; }
  public string? PropertyName { get; }

  public FindRolesQuery(TenantId? tenantId, IEnumerable<string> roles, string? propertyName = null)
    : this(tenantId, roles.Select(role => new RoleModification(role)), propertyName)
  {
  }

  public FindRolesQuery(TenantId? tenantId, IEnumerable<RoleModification> roles, string? propertyName = null)
  {
    TenantId = tenantId;
    Roles = roles.ToArray();
    PropertyName = propertyName;
  }
}

internal class FindRolesQueryHandler : IRequestHandler<FindRolesQuery, IReadOnlyCollection<FoundRole>>
{
  private readonly IRoleRepository _roleRepository;

  public FindRolesQueryHandler(IRoleRepository roleRepository)
  {
    _roleRepository = roleRepository;
  }

  public async Task<IReadOnlyCollection<FoundRole>> Handle(FindRolesQuery query, CancellationToken cancellationToken)
  {
    int capacity = query.Roles.Count;
    Dictionary<RoleId, FoundRole> foundRoles = new(capacity);
    HashSet<string> missingRoles = new(capacity);

    IReadOnlyCollection<Role> roles = await _roleRepository.LoadAsync(query.TenantId, cancellationToken);
    capacity = roles.Count;
    Dictionary<Guid, Role> rolesById = new(capacity);
    Dictionary<string, Role> rolesByUniqueName = new(capacity);
    foreach (Role role in roles)
    {
      rolesById[role.EntityId.ToGuid()] = role;
      rolesByUniqueName[role.UniqueName.Value.ToUpperInvariant()] = role;
    }

    foreach (RoleModification modification in query.Roles)
    {
      if (!string.IsNullOrWhiteSpace(modification.Role))
      {
        string trimmed = modification.Role.Trim();
        if (Guid.TryParse(trimmed, out Guid id) && rolesById.TryGetValue(id, out Role? role)
          || rolesByUniqueName.TryGetValue(trimmed.ToUpperInvariant(), out role))
        {
          foundRoles[role.Id] = new FoundRole(role, modification.Action);
        }
        else
        {
          missingRoles.Add(modification.Role);
        }
      }
    }

    if (missingRoles.Count > 0)
    {
      throw new RolesNotFoundException(missingRoles, query.PropertyName);
    }

    return foundRoles.Values;
  }
}
