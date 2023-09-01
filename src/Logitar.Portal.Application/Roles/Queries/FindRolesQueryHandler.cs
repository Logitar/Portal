using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal class FindRolesQueryHandler : IRequestHandler<FindRolesQuery, IEnumerable<RoleAggregate>>
{
  private readonly IRoleRepository _roleRepository;

  public FindRolesQueryHandler(IRoleRepository roleRepository)
  {
    _roleRepository = roleRepository;
  }

  public async Task<IEnumerable<RoleAggregate>> Handle(FindRolesQuery query, CancellationToken cancellationToken)
  {
    int roleCount = query.IdOrUniqueNames.Count();
    if (roleCount == 0)
    {
      return Enumerable.Empty<RoleAggregate>();
    }

    IEnumerable<RoleAggregate> roles = await _roleRepository.LoadAsync(query.Realm, cancellationToken);
    Dictionary<Guid, RoleAggregate> rolesById = new(capacity: roles.Count());
    Dictionary<string, RoleAggregate> rolesByUniqueName = new(capacity: rolesById.Count);
    foreach (RoleAggregate role in roles)
    {
      rolesById[role.Id.ToGuid()] = role;
      rolesByUniqueName[role.UniqueName.ToUpper()] = role;
    }

    List<RoleAggregate> foundRoles = new(capacity: roleCount);
    List<string> missingRoles = new(capacity: roleCount);

    foreach (string idOrUniqueName in query.IdOrUniqueNames)
    {
      if (!string.IsNullOrWhiteSpace(idOrUniqueName))
      {
        if ((Guid.TryParse(idOrUniqueName.Trim(), out Guid id) && rolesById.TryGetValue(id, out RoleAggregate? role))
          || rolesByUniqueName.TryGetValue(idOrUniqueName.Trim().ToUpper(), out role))
        {
          foundRoles.Add(role);
        }
        else
        {
          missingRoles.Add(idOrUniqueName);
        }
      }
    }

    if (missingRoles.Any())
    {
      throw new RolesNotFoundException(missingRoles, query.PropertyName);
    }

    return foundRoles.AsReadOnly();
  }
}
