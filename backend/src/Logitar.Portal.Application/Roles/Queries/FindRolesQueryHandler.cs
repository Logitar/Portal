using Logitar.Identity.Domain.Roles;
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
    IEnumerable<RoleAggregate> allRoles = await _roleRepository.LoadAsync(query.TenantId, cancellationToken);
    int capacity = allRoles.Count();
    Dictionary<string, RoleAggregate> rolesById = new(capacity);
    Dictionary<string, RoleAggregate> rolesByUniqueName = new(capacity);
    foreach (RoleAggregate role in allRoles)
    {
      rolesById[role.Id.Value] = role;
      rolesByUniqueName[role.UniqueName.Value.ToUpper()] = role;
    }

    Dictionary<RoleId, RoleAggregate> roles = new(capacity);
    List<string> missingRoles = new(capacity);
    foreach (string value in query.Roles)
    {
      string id = value.Trim();
      string uniqueNameNormalized = id.ToUpper();
      if (rolesById.TryGetValue(id, out RoleAggregate? role) || rolesByUniqueName.TryGetValue(uniqueNameNormalized, out role))
      {
        roles[role.Id] = role;
      }
      else
      {
        missingRoles.Add(value);
      }
    }

    if (missingRoles.Count > 0)
    {
      throw new RolesNotFoundException(missingRoles, query.PropertyName);
    }

    return roles.Values;
  }
}
