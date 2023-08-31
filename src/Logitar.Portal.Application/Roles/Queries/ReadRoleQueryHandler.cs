using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal class ReadRoleQueryHandler : IRequestHandler<ReadRoleQuery, Role?>
{
  private readonly IRoleQuerier _roleQuerier;

  public ReadRoleQueryHandler(IRoleQuerier roleQuerier)
  {
    _roleQuerier = roleQuerier;
  }

  public async Task<Role?> Handle(ReadRoleQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Role> roles = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Role? role = await _roleQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (query.UniqueName != null)
    {
      Role? role = await _roleQuerier.ReadAsync(query.Realm, query.UniqueName, cancellationToken);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (roles.Count > 1)
    {
      throw new TooManyResultsException<Role>(expected: 1, actual: roles.Count);
    }

    return roles.Values.SingleOrDefault();
  }
}
