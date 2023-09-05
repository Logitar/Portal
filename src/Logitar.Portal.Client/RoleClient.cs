using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Client;

internal class RoleClient : ClientBase, IRoleService
{
  private const string Path = "/api/roles";

  public RoleClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Role> CreateAsync(CreateRolePayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<Role>(Path, payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<Role?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await DeleteAsync<Role>($"{Path}/{id}", cancellationToken);
  }

  public async Task<Role?> ReadAsync(Guid? id, string? realm, string? uniqueName, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Role> roles = new(capacity: 2);

    if (id.HasValue)
    {
      Role? role = await GetAsync<Role>($"{Path}/{id}", cancellationToken);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (uniqueName != null)
    {
      StringBuilder path = new();

      path.Append(Path).Append("/unique-name:").Append(uniqueName);
      if (realm != null)
      {
        path.Append("?realm=").Append(realm);
      }

      Role? role = await GetAsync<Role>(path.ToString(), cancellationToken);
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

  public async Task<Role?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version, CancellationToken cancellationToken)
  {
    StringBuilder path = new();

    path.Append(Path).Append('/').Append(id);

    if (version.HasValue)
    {
      path.Append("?version=").Append(version.Value);
    }

    return await PutAsync<Role>(path.ToString(), payload, cancellationToken);
  }

  public async Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SearchResults<Role>>($"{Path}/search", payload, cancellationToken)
      ?? throw new InvalidOperationException("The results should not be null.");
  }

  public async Task<Role?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    return await PatchAsync<Role>($"{Path}/{id}", payload, cancellationToken);
  }
}
