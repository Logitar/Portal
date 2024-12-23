using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Client.Users;

internal class UserClient : BaseClient, IUserClient
{
  private const string Path = "/api/users";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public UserClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<User> AuthenticateAsync(AuthenticateUserPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/authenticate", UriKind.Relative);
    return await PatchAsync<User>(uri, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(AuthenticateAsync), HttpMethod.Patch, uri, payload, context);
  }

  public async Task<User> CreateAsync(CreateUserPayload payload, IRequestContext? context)
  {
    return await PostAsync<User>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<User?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<User>(uri, context);
  }

  public async Task<User?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifierModel? identifier, IRequestContext? context)
  {
    Dictionary<Guid, User> users = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      User? user = await GetAsync<User>(uri, context);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      Uri uri = new($"{Path}/unique-name:{uniqueName}", UriKind.Relative);
      User? user = await GetAsync<User>(uri, context);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (identifier != null)
    {
      Uri uri = new($"{Path}/identifier/key:{identifier.Key}/value:{identifier.Value}", UriKind.Relative);
      User? user = await GetAsync<User>(uri, context);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw new TooManyResultsException<User>(expectedCount: 1, actualCount: users.Count);
    }

    return users.Values.SingleOrDefault();
  }

  public async Task<User?> RemoveIdentifierAsync(Guid id, string key, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/identifiers/key:{key}", UriKind.Relative);
    return await DeleteAsync<User>(uri, context);
  }

  public async Task<User?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<User>(uri, payload, context);
  }

  public async Task<User?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/password/reset", UriKind.Relative);
    return await PatchAsync<User>(uri, payload, context);
  }

  public async Task<User?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/identifiers/key:{key}", UriKind.Relative);
    return await PutAsync<User>(uri, payload, context);
  }

  public async Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (payload.HasAuthenticated.HasValue)
    {
      builder.SetQuery("has_authenticated", payload.HasAuthenticated.Value.ToString());
    }
    if (payload.HasPassword.HasValue)
    {
      builder.SetQuery("has_password", payload.HasPassword.Value.ToString());
    }
    if (payload.IsDisabled.HasValue)
    {
      builder.SetQuery("disabled", payload.IsDisabled.Value.ToString());
    }
    if (payload.IsConfirmed.HasValue)
    {
      builder.SetQuery("confirmed", payload.IsConfirmed.Value.ToString());
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<User>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<User?> SignOutAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/sign/out", UriKind.Relative);
    return await PutAsync<User>(uri, content: null, context);
  }

  public async Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<User>(uri, payload, context);
  }
}
