using Logitar.Identity.Domain.Roles;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Roles;

public interface IRoleQuerier
{
  Task<Role> ReadAsync(Realm? realm, RoleAggregate role, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(Realm? realm, RoleId id, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(Realm? realm, string uniqueName, CancellationToken cancellationToken = default);
  Task<SearchResults<Role>> SearchAsync(Realm? realm, SearchRolesPayload payload, CancellationToken cancellationToken = default);
}
