using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal class ReadRoleQueryHandler : IRequestHandler<ReadRoleQuery, RoleModel?>
{
  private readonly IRoleQuerier _roleQuerier;

  public ReadRoleQueryHandler(IRoleQuerier roleQuerier)
  {
    _roleQuerier = roleQuerier;
  }

  public async Task<RoleModel?> Handle(ReadRoleQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, RoleModel> roles = new(capacity: 2);

    if (query.Id.HasValue)
    {
      RoleModel? role = await _roleQuerier.ReadAsync(query.Realm, query.Id.Value, cancellationToken);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      RoleModel? role = await _roleQuerier.ReadAsync(query.Realm, query.UniqueName, cancellationToken);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (roles.Count > 1)
    {
      throw new TooManyResultsException<RoleModel>(expectedCount: 1, actualCount: roles.Count);
    }

    return roles.Values.SingleOrDefault();
  }
}
