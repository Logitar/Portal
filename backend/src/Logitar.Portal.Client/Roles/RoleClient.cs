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

  public async Task<RoleModel> CreateAsync(CreateRolePayload payload, IRequestContext? context)
  {
    return await PostAsync<RoleModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<RoleModel?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<RoleModel>(uri, context);
  }

  public async Task<RoleModel?> ReadAsync(Guid? id, string? uniqueName, IRequestContext? context)
  {
    Dictionary<Guid, RoleModel> roles = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      RoleModel? role = await GetAsync<RoleModel>(uri, context);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      Uri uri = new($"{Path}/unique-name:{uniqueName}", UriKind.Relative);
      RoleModel? role = await GetAsync<RoleModel>(uri, context);
      if (role != null)
      {
        roles[role.Id] = role;
      }
    }

    if (roles.Count > 1)
    {
      throw TooManyResultsException<RoleModel>.ExpectedSingle(roles.Count);
    }

    return roles.Values.SingleOrDefault();
  }

  public async Task<RoleModel?> ReplaceAsync(Guid id, ReplaceRolePayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<RoleModel>(uri, payload, context);
  }

  public async Task<SearchResults<RoleModel>> SearchAsync(SearchRolesPayload payload, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath(Path).SetQuery(payload).BuildUri(UriKind.Relative);
    return await GetAsync<SearchResults<RoleModel>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<RoleModel?> UpdateAsync(Guid id, UpdateRolePayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<RoleModel>(uri, payload, context);
  }
}
