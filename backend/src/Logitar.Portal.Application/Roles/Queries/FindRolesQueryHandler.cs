using Logitar.Identity.Domain.Roles;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal class FindRolesQueryHandler : IRequestHandler<FindRolesQuery, IEnumerable<FoundRole>>
{
  private readonly IRoleRepository _roleRepository;

  public FindRolesQueryHandler(IRoleRepository roleRepository)
  {
    _roleRepository = roleRepository;
  }

  public async Task<IEnumerable<FoundRole>> Handle(FindRolesQuery query, CancellationToken cancellationToken)
  {
    int capacity = query.Roles.Count();
    Dictionary<RoleId, FoundRole> foundRoles = new(capacity);
    HashSet<string> missingRoles = new(capacity);

    IEnumerable<RoleAggregate> roles = await _roleRepository.LoadAsync(query.TenantId, cancellationToken);
    capacity = roles.Count();
    Dictionary<Guid, RoleAggregate> rolesById = new(capacity);
    Dictionary<string, RoleAggregate> rolesByUniqueName = new(capacity);
    foreach (RoleAggregate role in roles)
    {
      rolesById[role.Id.ToGuid()] = role;
      rolesByUniqueName[role.UniqueName.Value.ToUpper()] = role;
    }

    foreach (RoleModification modification in query.Roles)
    {
      if (!string.IsNullOrWhiteSpace(modification.Role))
      {
        string trimmed = modification.Role.Trim();
        if (Guid.TryParse(trimmed, out Guid id) && rolesById.TryGetValue(id, out RoleAggregate? role)
          || rolesByUniqueName.TryGetValue(trimmed.ToUpper(), out role))
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
