using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Client.Roles;

internal class RoleClient : BaseClient, IRoleClient
{
  private const string Path = "/api/roles";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public RoleClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Role> CreateAsync(CreateRolePayload payload, IRequestContext? context)
  {
    return await PostAsync<Role>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<Role?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<Role>(uri, context);
  }

  public async Task<Role?> ReadAsync(Guid? id, string? uniqueName, IRequestContext? context)
  {
    Dictionary<Guid, Role> roles = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      Role? role = await GetAsync<Role>(uri, context);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      Uri uri = new($"{Path}/unique-name:{uniqueName}", UriKind.Relative);
      Role? role = await GetAsync<Role>(uri, context);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (roles.Count > 1)
    {
      throw new TooManyResultsException<Role>(expectedCount: 1, actualCount: roles.Count);
    }

    return roles.Values.SingleOrDefault();
  }

  public async Task<Role?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<Role>(uri, payload, context);
  }

  public async Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath(Path).SetQuery(payload).BuildUri(UriKind.Relative);
    return await GetAsync<SearchResults<Role>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<Role?> UpdateAsync(Guid id, UpdateRolePayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<Role>(uri, payload, context);
  }
}
