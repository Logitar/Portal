using Logitar.Identity.Core.Roles;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Roles;

public interface IRoleQuerier
{
  Task<RoleModel> ReadAsync(RealmModel? realm, Role role, CancellationToken cancellationToken = default);
  Task<RoleModel?> ReadAsync(RealmModel? realm, RoleId id, CancellationToken cancellationToken = default);
  Task<RoleModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<RoleModel?> ReadAsync(RealmModel? realm, string uniqueName, CancellationToken cancellationToken = default);
  Task<SearchResults<RoleModel>> SearchAsync(RealmModel? realm, SearchRolesPayload payload, CancellationToken cancellationToken = default);
}
