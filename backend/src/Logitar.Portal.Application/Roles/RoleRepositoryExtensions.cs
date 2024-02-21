using Logitar.Identity.Domain.Roles;

namespace Logitar.Portal.Application.Roles;

internal static class RoleRepositoryExtensions
{
  public static async Task<RoleAggregate?> LoadAsync(this IRoleRepository repository, Guid id, CancellationToken cancellationToken = default)
  {
    RoleId roleId = new(id);
    return await repository.LoadAsync(roleId, cancellationToken);
  }
}
